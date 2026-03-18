using Xceed.Words.NET;
using KTGK.Models;
using System.Text.RegularExpressions;

namespace KTGK.Parsers
{
    public class ParsedExam
    {
        public string Title { get; set; }
        public int Duration { get; set; }
        public List<ParsedQuestion> Questions { get; set; } = new();
        public List<ParsedPassage> Passages { get; set; } = new();
    }

    public class ParsedPassage
    {
        public int TempId { get; set; }
        public string Content { get; set; }
    }

    public class ParsedQuestion
    {
        public int Number { get; set; }
        public string Content { get; set; }
        public List<string> Options { get; set; } = new();
        public int CorrectIndex { get; set; }
        public int? PassageTempId { get; set; }
        public int Part { get; set; }
    }

    public class ToeicWordParser
    {
        public ParsedExam Parse(string filePath)
        {
            var result = new ParsedExam();
            var doc = DocX.Load(filePath);

            // ✅ KEY FIX: Tách theo \n bên trong mỗi paragraph
            var lines = doc.Paragraphs
                .SelectMany(p => p.Text.Split('\n'))
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            int currentPart = 5;
            int passageTempId = 0;
            bool inPassage = false;
            var passageLines = new List<string>();
            ParsedQuestion currentQ = null;

            foreach (var line in lines)
            {
                // EXAM TITLE
                if (line.StartsWith("[EXAM_TITLE]"))
                {
                    result.Title = line.Replace("[EXAM_TITLE]", "").Trim();
                    continue;
                }

                // EXAM TIME
                if (line.StartsWith("[EXAM_TIME]"))
                {
                    if (int.TryParse(line.Replace("[EXAM_TIME]", "").Trim(), out int dur))
                        result.Duration = dur;
                    continue;
                }

                // PART
                var partMatch = Regex.Match(line, @"\[PART:\s*(\d)\]");
                if (partMatch.Success)
                {
                    currentPart = int.Parse(partMatch.Groups[1].Value);
                    continue;
                }

                // Bỏ qua dòng Directions
                if (line.StartsWith("Directions:"))
                    continue;

                // PASSAGE START
                if (line == "[PASSAGE_START]")
                {
                    inPassage = true;
                    passageLines.Clear();
                    passageTempId++;
                    continue;
                }

                // PASSAGE END
                if (line == "[PASSAGE_END]")
                {
                    inPassage = false;
                    result.Passages.Add(new ParsedPassage
                    {
                        TempId = passageTempId,
                        Content = string.Join("\n", passageLines)
                    });
                    continue;
                }

                // Gom nội dung passage
                if (inPassage)
                {
                    passageLines.Add(line);
                    continue;
                }

                // QUESTION — dòng bắt đầu bằng [Q:xxx]
                var qMatch = Regex.Match(line, @"\[Q:(\d+)\]");
                if (qMatch.Success)
                {
                    if (currentQ != null)
                        result.Questions.Add(currentQ);

                    // Lấy nội dung sau tag [Q:xxx] [SHUFFLE:xxx]
                    var content = Regex.Replace(line, @"\[Q:\d+\]\s*(\[SHUFFLE:\w+\])?\s*", "").Trim();

                    currentQ = new ParsedQuestion
                    {
                        Number = int.Parse(qMatch.Groups[1].Value),
                        Content = content,
                        Part = currentPart,
                        PassageTempId = currentPart >= 6 ? passageTempId : null
                    };
                    continue;
                }

                // Nội dung câu hỏi nhiều dòng (trước khi có options)
                if (currentQ != null && currentQ.Options.Count == 0
                    && !Regex.IsMatch(line, @"^\[([ABCD])\]")
                    && !line.StartsWith("[KEY:"))
                {
                    if (string.IsNullOrEmpty(currentQ.Content))
                        currentQ.Content = line;
                    else
                        currentQ.Content += " " + line;
                    continue;
                }

                // OPTIONS [A] [B] [C] [D]
                if (currentQ != null)
                {
                    var optMatch = Regex.Match(line, @"^\[([ABCD])\]\s*(.*)$");
                    if (optMatch.Success)
                    {
                        currentQ.Options.Add(optMatch.Groups[2].Value.Trim());
                        continue;
                    }

                    // KEY
                    var keyMatch = Regex.Match(line, @"\[KEY:([ABCD])\]");
                    if (keyMatch.Success)
                    {
                        currentQ.CorrectIndex = keyMatch.Groups[1].Value[0] - 'A';
                        result.Questions.Add(currentQ);
                        currentQ = null;
                        continue;
                    }
                }
            }

            if (currentQ != null)
                result.Questions.Add(currentQ);

            return result;
        }
    }
}