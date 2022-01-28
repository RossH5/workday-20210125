using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quizzappy.Models
{
    public class MultipleChoiceQuestion : Question
    {
        [Required]
        public virtual List<TextAnswer> Answers { get; set; }
        public virtual TextAnswer CorrectAnswer { get; set; }
    }
}
