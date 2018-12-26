function newMessage(pUser, pTime, pText) {
	var width = window.innerWidth
		|| document.documentElement.clientWidth
		|| document.body.clientWidth;

	var height = window.innerHeight
		|| document.documentElement.clientHeight
		|| document.body.clientHeight;
	
	
	var msg = document.createElement("div");
	msg.className = "message";
	if(pUser != "") {
		var info = document.createElement("div");
		info.className = "info";
		var user = document.createElement("div");
		user.className = "user";
		user.innerText = pUser;
		info.appendChild(user);
		var time =  document.createElement("div");
		time.className = "time";
		time.innerText = pTime;
		info.appendChild(time);
		msg.appendChild(info);
	}
	var content = document.createElement("div");
	content.className = "content";
	content.innerHTML = pText;
	msg.appendChild(content);
	document.getElementById("container").appendChild(msg);
	
	var imgs = document.getElementsByTagName("*");
	for(var i = 0; i<imgs.length; i++)
	{
		if(imgs[i].className == "msg-img") {
			var ow = imgs[i].offsetWidth;
			var oh = imgs[i].offsetHeight;
			
			if(ow > width) {
				imgs[i].style.width = ""+(width*90/100);
				imgs[i].style.height = "auto";
			}
		}
	}
	
	window.scrollTo(0,document.body.scrollHeight);
}
