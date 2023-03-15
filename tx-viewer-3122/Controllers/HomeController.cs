using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using tx_viewer_sign.Models;
using TXTextControl;

namespace tx_viewer_sign.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {

         FormData data = new FormData() {
            patient = new Patient() {
               Name = "Typer",
               Firstname = "Tim",
               DOB = DateTime.Now,
               City = "Charlotte",
               Gender = "Male",
               State = "North Carolina",
               Street = "1 Text Control Ave",
               ZIP = "28209"
            }
         };

            return View(data);
        }

		private void FlattenFormFields(ServerTextControl textControl) {
			int fieldCount = textControl.FormFields.Count;

			for (int i = 0; i < fieldCount; i++) {
				TextFieldCollectionBase.TextFieldEnumerator fieldEnum =
				  textControl.FormFields.GetEnumerator();
				fieldEnum.MoveNext();

				FormField curField = (FormField)fieldEnum.Current;
				textControl.FormFields.Remove(curField, true);
			}
		}

		public IActionResult Create(FormData data) {


			SignData signData = new SignData();

			byte[] document;

            using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl()) {
               tx.Create();
               tx.Load("App_Data/vaccination_form.tx", TXTextControl.StreamType.InternalUnicodeFormat);
               
               using (TXTextControl.DocumentServer.MailMerge mm = new TXTextControl.DocumentServer.MailMerge()) {
                  mm.TextComponent = tx;
                  mm.FormFieldMergeType = TXTextControl.DocumentServer.FormFieldMergeType.Preselect;
                  mm.RemoveEmptyFields = false;
                  mm.MergeObject(data);
               }

               tx.Save(out document, TXTextControl.BinaryStreamType.InternalUnicodeFormat);

            }

			signData.document = document;
         signData.formData = data;


				return View(signData);
		   }

		[HttpPost]
		public string ExportPDF([FromBody] TXTextControl.Web.MVC.DocumentViewer.Models.SignatureData data) {

			byte[] bPDF;

			// create temporary ServerTextControl
			using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl()) {
				tx.Create();

				// load the document
				tx.Load(Convert.FromBase64String(data.SignedDocument.Document), TXTextControl.BinaryStreamType.InternalUnicodeFormat);

            FlattenFormFields(tx);

				// save the document as PDF
				tx.Save(out bPDF, TXTextControl.BinaryStreamType.AdobePDFA);
			}

         return Convert.ToBase64String(bPDF);
		}



		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}