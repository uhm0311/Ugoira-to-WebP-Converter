var ugoira_zip = function (currentPosition, ugoiraCount) {
	var title = document.querySelector(".work-info .title");
	title.appendChild(document.createElement("br"));
	
	[{
		name: "Zip",
		data: pixiv.context.ugokuIllustData
	}, {
		name: "ZipHQ",
		data: pixiv.context.ugokuIllustFullscreenData
	}].forEach(function (value) {
		var elem, elemtxt, click;
		click = function () {
			if ("removeEventListener" in elem) {
				elem.removeEventListener("click", click);
			} else {
				elem.detachEvent("onclick", click);
			}
			
			var basename = pixiv.context.illustId + "_" + pixiv.context.illustTitle.replace(/[\\/:*?"<>|\u00b7]/g, "");
			var savename = basename + value.name + ".zip";
			elemtxt.nodeValue = " >Loading" + value.name + "...< ";
			
			if (typeof window.external.onUgoiraDownloadStarted != 'undefined') {
				window.external.onUgoiraDownloadStarted(savename, currentPosition, ugoiraCount);
			}
			
			var xhr = new XMLHttpRequest();
			xhr.open("GET", value.data.src);
			xhr.responseType = "arraybuffer";
			xhr.addEventListener("load", function () {
				elemtxt.nodeValue = " >Save" + value.name + "< ";
				var zip = new JSZip(xhr.response);
				zip.file("animation.json", JSON.stringify(pixiv.context));
				
				if (typeof window.external.onUgoiraDownloadPerformed != 'undefined') {
					window.external.onUgoiraDownloadPerformed(savename, zip.generate({ type: "base64" }), currentPosition, ugoiraCount);
				}
			});
			xhr.addEventListener("error", function () {
				elemtxt.nodeValue = " >Error" + value.name + "< ";
				
				if (typeof window.external.onUgoiraDownloadFailed != 'undefined') {
					window.external.onUgoiraDownloadFailed(savename, currentPosition, ugoiraCount);
				}
			});
			xhr.send();
		};
		elem = document.createElement("a");
		elemtxt = document.createTextNode(" >DL" + value.name + "< ");
		elem.appendChild(elemtxt);
		elem.className = "_button";
		elem.setAttribute("id", "DL" + value.name);
		
		if ("addEventListener" in elem) {
			elem.addEventListener("click", click);
		} else {
			elem.attachEvent("onclick", click);
		}
		
		title.appendChild(elem);		
		if (value.name == "ZipHQ") {
			elem.click();
		}
	});
};