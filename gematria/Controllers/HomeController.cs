using gematria.Models;
using gematria.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace gematria.Controllers
{
    public class HomeController : Controller
    {
        private readonly TextService _textService;
        private readonly LanguageService _langService;
        private readonly GematriaService _gematriaService;
        private readonly IWebHostEnvironment _environment;

        string _path;
        string _letters = null;
        List<int> _values = new List<int>();

        public HomeController(TextService textService, LanguageService langService, GematriaService gematriaService, IWebHostEnvironment environment)
        {
            _textService = textService;
            _langService = langService;
            _gematriaService = gematriaService;
            _environment = environment;
            _path = Path.Combine(_environment.ContentRootPath, "Data", "searches.csv");
        }

        public IActionResult Index()
        {
            string lang = "en";
            if (Request.Query.ContainsKey("lang"))
            {
                lang = Request.Query["lang"];
            }
            _letters = _langService.Get(lang);
            ViewBag.Letters = _letters;

            List<SelectListItem> languages = new List<SelectListItem>();
            languages.Add(new SelectListItem("English", "en"));
            SelectListItem language = languages.FirstOrDefault(x => x.Value == lang);
            if (language != null)
            {
                language.Selected = true;
            }
            ViewBag.Languages = languages;

            int startValue = 1;
            int stepValue = 1;

            if (Request.Query.ContainsKey("start") && Request.Query.ContainsKey("step"))
            {
                string start = Request.Query["start"];
                string step = Request.Query["step"];
                int.TryParse(start, out startValue);
                int.TryParse(step, out stepValue);
            }

            for (int i = 0; i < _letters.Count(); i++)
            {
                _values.Add(startValue);
                startValue += stepValue;
            }
            ViewBag.Values = _values;

            if (Request.Query.ContainsKey("q"))
            {
                string q = Request.Query["q"].ToString().ToUpper();

                List<int> queryValues = GetGematria(q);

                var sum = queryValues.Sum();
                string result = string.Join(" + ", queryValues) + $" = {sum}";
                ViewBag.Result = result;

                if (sum == 666)
                {
                    _textService.Write(_path, string.Format("{0}, {1}", q.Trim(), result));
                }
            }

            return View();
        }

        private List<int> GetGematria(string q)
        {
            List<int> queryValues = new List<int>();

            foreach (char queryLetter in q)
            {
                if (char.IsLetter(queryLetter))
                {
                    if (_letters.Contains(queryLetter))
                    {
                        int queryIndex = _letters.IndexOf(queryLetter);
                        int queryValue = _values[queryIndex];
                        queryValues.Add(queryValue);
                    }
                }
            }

            return queryValues;
        }

        public IActionResult Searches()
        {
            List<string> lines = _textService.Read(_path);
            List<Phrase> phrases = lines.Select(x =>
            {
                List<string> splits = x.ToUpper().Split(',').Select(y => y.Trim()).ToList();
                return new Phrase()
                {
                    Text = splits.FirstOrDefault(),
                    Value = splits.Count > 1 ? splits[1] : "",
                    Type = splits.Count > 2 ? splits[2] : ""
                };
            }).ToList();

            return View(phrases.OrderBy(x => x.Type).ThenBy(x => x.Text).ToList());
        }

        public IActionResult Words()
        {
            List<BookNumber> model = new List<BookNumber>();

            string fileName = Path.Combine(_environment.ContentRootPath, "Data", "Strongs Numbers.xlsx");

            FileInfo fileInfo = new FileInfo(fileName);
            ExcelPackage package = new ExcelPackage(fileInfo);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.LastOrDefault();
            int totalRows = worksheet.Dimension.Rows;

            for (int rowNumber = 1; rowNumber <= totalRows; rowNumber++)
            {
                BookNumber item = new BookNumber();
                int.TryParse(string.Format("{0}", worksheet.Cells[rowNumber, 1].Value), out int number);
                item.Number = number;
                item.Book = string.Format("{0}", worksheet.Cells[rowNumber, 2].Value);
                model.Add(item);
            }

            return View(model);
        }
    }
}
