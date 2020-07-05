var ugoira_data = function() {
	var illustIDPrefix = 'artworks/';
	var illustID = document.location.href.substring(document.location.href.indexOf(illustIDPrefix) + illustIDPrefix.length);

	var metaDataPrefix = 'https://www.pixiv.net/ajax/illust/';
	var metaDataSuffix = '/ugoira_meta';

	var xhr1 = new XMLHttpRequest();
	xhr1.open("get", metaDataPrefix + illustID, false);
	xhr1.send();
	
	var xhr2 = new XMLHttpRequest();
	xhr2.open("get", metaDataPrefix + illustID + metaDataSuffix, false);
	xhr2.send();
	
	var ugoiraObj = {};
	var metaData = JSON.parse(xhr1.response);
	var ugoiraMetaData = JSON.parse(xhr2.response);
	
	ugoiraObj['animation.json'] = JSON.stringify({ ugokuIllustData: ugoiraMetaData.body });
	ugoiraObj['url'] = ugoiraMetaData.body.originalSrc;
	ugoiraObj['zip'] = illustID + "_" + metaData.body.illustTitle.replace(/[\\/:*?"<>|\u00b7]/g, "") + "ZipHQ.zip";
	ugoiraObj['userName'] = metaData.body.userName;

	return JSON.stringify(ugoiraObj);
};