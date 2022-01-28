using Quizzappy.Daos;
using Quizzappy.Extensions;
using Quizzappy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quizzappy
{
    public static class QuizSeed
    {
        public static void InitData(QuizContext context)
        {
            var rnd = new Random();

            var beginningQuestionText = new[] { "What does", "How many", "What is the difference between" };
            var endQuestionText = new[] { "a fox and a bull", "this is totally a real question", "how do I make randomly generated questions"};

            var totalQuestions = 0;
            var totalAnswers = 1;

            context.Quizzes.AddRange(5.Times(x =>
            {
                var quizName = $"Quiz {x,-3:000}";
                int totalScore = 0;


                var questionLength = rnd.Next(3, 10);
                var multipleChoiceQuestions = new List<MultipleChoiceQuestion>();
                var shortAnswerQuestions = new List<ShortAnswerQuestion>();
                var fillTheBlanksQuestions = new List<FillTheBlanksQuestion>();
                
                for(int i = 0; i < questionLength; i++)
                {
                    var beginningQuestion = beginningQuestionText[rnd.Next(0, 3)];
                    var endQuestion = endQuestionText[rnd.Next(0, 3)];

                    totalQuestions += 1;
                    var typeOfQuestion = rnd.Next(0, 3);
                    switch(typeOfQuestion)
                    {
                        case 0:
                            {
                                var multipleChoiceAnswers = new List<TextAnswer>
                                {
                                    new TextAnswer
                                    {
                                        AnswerId = totalAnswers,
                                        Answer = "A"
                                    },
                                    new TextAnswer
                                    {
                                        AnswerId = totalAnswers + 1,
                                        Answer = "B"
                                    },
                                    new TextAnswer
                                    {
                                        AnswerId = totalAnswers + 2,
                                        Answer = "C"
                                    },
                                    new TextAnswer
                                    {
                                        AnswerId = totalAnswers + 3,
                                        Answer = "D"
                                    },
                                    new TextAnswer
                                    {
                                        AnswerId = totalAnswers + 4,
                                        Answer = "E"
                                    }
                                };

                                totalAnswers += 5;
                                int answerindex = rnd.Next(1, 4);
                                TextAnswer finalAnswer = multipleChoiceAnswers[answerindex];

                                var question = new MultipleChoiceQuestion
                                {
                                    QuestionId = totalQuestions,
                                    QuestionText = beginningQuestion + " " + endQuestion + $" ?",
                                    Answers = multipleChoiceAnswers,
                                    CorrectAnswer = finalAnswer,
                                    Score = 1
                                };

                                totalScore += question.Score;

                                multipleChoiceQuestions.Add(question);
                                break;
                            }
                        case 1:
                            {
                                var question = new ShortAnswerQuestion
                                {
                                    QuestionId = totalQuestions,
                                    QuestionText = beginningQuestion + " " + endQuestion + $" ?",
                                    WordLimit = rnd.Next(50, 500),
                                    Score = 5
                                };

                                totalScore += question.Score;
                                shortAnswerQuestions.Add(question);
                                break;
                            }
                        case 2:
                            {
                                var fillTheBlanksAnswers = new List<TextAnswer>()
                                    {
                                        new TextAnswer
                                        {
                                            AnswerId = totalAnswers,
                                            Answer = "true"
                                        },
                                        new TextAnswer
                                        {
                                            AnswerId = totalAnswers+1,
                                            Answer = "false"
                                        }
                                    };

                                totalAnswers += 2;

                                var question = new FillTheBlanksQuestion
                                {
                                    CorrectAnswers = fillTheBlanksAnswers,
                                    Score = 2
                                    
                                };

                                totalScore += question.Score;
                                fillTheBlanksQuestions.Add(question);
                                break;
                            }
                        default: break;

                    }
                }

                return new Quiz()
                {
                    QuizName = quizName,
                    QuizId = x,
                    QuizScore = totalScore,
                    MultipleChoiceQuestions = multipleChoiceQuestions,
                    ShortAnswerQuestions = shortAnswerQuestions,
                    FillTheBlanksQuestions = fillTheBlanksQuestions

                };
            }));

            context.SaveChanges();
        }
    }
}
