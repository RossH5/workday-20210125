using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quizzappy.Daos;
using Quizzappy.Models;

namespace Quizzappy.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class QuizController
    {
        private readonly QuizContext _context;
        public QuizController(QuizContext context)
        {
            _context = context;

            if (_context.Quizzes.Any()) return;

            QuizSeed.InitData(context);
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IQueryable<Quiz>> GetQuizzes()
        {
            var result = _context.Quizzes as IQueryable<Quiz>;

            return new OkObjectResult(result
                .OrderBy(q => q.QuizId));
        }

        [HttpDelete]
        [Route("{quizId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IQueryable<Quiz>> DeleteQuiz([FromRoute] int quizId)
        {
            try
            {
                var quizList = _context.Quizzes as IQueryable<Quiz>;
                var quiz = quizList.First(q => q.QuizId.Equals(quizId));

                _context.Quizzes.Remove(quiz);
                _context.SaveChanges();

                return new OkObjectResult(quizId);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [HttpPut]
        [Route("{quizId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IQueryable<Quiz>> PutQuiz([FromRoute] int quizId, [FromBody] Quiz newQuiz)
        {
            try
            {
                var quizList = _context.Quizzes as IQueryable<Quiz>;
                var quiz = quizList.FirstOrDefault(q => q.QuizId.Equals(quizId));

                if (quiz != null)
                {
                    _context.Quizzes.Remove(quiz);
                    _context.SaveChanges();
                }
                _context.Quizzes.Add(newQuiz);

                return new CreatedResult($"/quizzes/{newQuiz.QuizId}", newQuiz);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IQueryable<Quiz>> PostQuiz([FromBody] Quiz newQuiz)
        {
            try
            {
                _context.Quizzes.Add(newQuiz);
                _context.SaveChanges();

                return new CreatedResult($"/quizzes/{newQuiz.QuizId}", newQuiz);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [HttpPatch]
        [Route("{quizId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IQueryable<Quiz>> PatchQuiz([FromRoute] int quizId, [FromBody] Quiz patchQuiz)
        {
            try
            {
                var quizList = _context.Quizzes as IQueryable<Quiz>;
                var quiz = quizList.FirstOrDefault(q => q.QuizId.Equals(quizId));

                quiz.FillTheBlanksQuestions = patchQuiz.FillTheBlanksQuestions ?? quiz.FillTheBlanksQuestions;
                quiz.MultipleChoiceQuestions = patchQuiz.MultipleChoiceQuestions ?? quiz.MultipleChoiceQuestions;
                quiz.ShortAnswerQuestions = patchQuiz.ShortAnswerQuestions ?? quiz.ShortAnswerQuestions;

                _context.Update(quiz);
                _context.SaveChanges();

                return new CreatedResult($"/quizzes/{patchQuiz.QuizId}", patchQuiz);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [HttpGet]
        [Route("{quizId}/grade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<int> GradeQuizzes([FromRoute] int quizId, [FromBody] List<List<TextAnswer>> answers)
        {
            try
            {
                var quizList = _context.Quizzes as IQueryable<Quiz>;
                var quiz = quizList.FirstOrDefault(q => q.QuizId.Equals(quizId));
                int averageScore = 0;

                int totalScore = 0;
                for (int i = 0; i < answers.Count; i++)
                {
                    for (int j = 0; j < quiz.MultipleChoiceQuestions.Count; j++)
                    {
                        if (answers[i][j] == quiz.MultipleChoiceQuestions[j].CorrectAnswer)
                        {
                            totalScore += quiz.MultipleChoiceQuestions[j].Score;
                        }
                    }

                    for (int j = 0; j < quiz.ShortAnswerQuestions.Count; j++)
                    {
                        if (answers[i][j].Answer.Length < quiz.ShortAnswerQuestions[j].WordLimit)
                        {
                            totalScore += quiz.ShortAnswerQuestions[j].Score;
                        }
                    }
                    for (int j = 0; j < quiz.FillTheBlanksQuestions.Count; j++)
                    {
                        bool allMatched = true;
                        for (int k = 0; k < quiz.FillTheBlanksQuestions[i].CorrectAnswers.Count; k++)
                        {
                            if (quiz.FillTheBlanksQuestions[j].CorrectAnswers[k] !=
                                answers[i][j])
                            {
                                allMatched = false;
                            }
                        }

                        if (allMatched)
                        {
                            totalScore += quiz.FillTheBlanksQuestions[j].Score;
                        }

                    }
                    averageScore += totalScore;

                }

                averageScore /= answers.Count;

                return new OkObjectResult(averageScore);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
