using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quizzappy.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public int QuizScore { get; set; }
        public virtual List<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public virtual List<ShortAnswerQuestion> ShortAnswerQuestions { get; set; }
        public virtual List<FillTheBlanksQuestion> FillTheBlanksQuestions { get; set; }
    }
}
