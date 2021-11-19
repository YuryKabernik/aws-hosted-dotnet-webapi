using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProfileSample.DAL;
using ProfileSample.Models;

namespace ProfileSample.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			var model = new List<ImageModel>();

			using (var context = new ProfileSampleEntities()) // Dispose the entity
			{
				// reduce number of database calls
				model = context.ImgSources.Take(20).Select(x => new ImageModel
				{
					Name = x.Name,
					Data = x.Data
				})
				.ToList(); // request execution starts here
			}

			return View(model);
		}

		public ActionResult Convert()
		{
			var files = Directory.GetFiles(Server.MapPath("~/Content/Img"), "*.jpg");

			using (var context = new ProfileSampleEntities())
			{
				// not sure if parallelization helps here
				var imgsRange = files
					.Select(file =>
					{
						var img = new ImgSource()
						{
							Name = Path.GetFileName(file)
						};

						using (var stream = new FileStream(file, FileMode.Open))
						{
							img.Data = new byte[stream.Length];
							stream.Read(img.Data, 0, (int)stream.Length);
						}

						return img;
					});

				context.ImgSources.AddRange(imgsRange); // add range instead of each element
				context.SaveChanges(); // move saving changes closer to the end of operation to reduce number of calls
			}

			return RedirectToAction("Index");
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}