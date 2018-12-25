function newMessage(pUser, pTime, pText) {
	var msg = document.createElement("div");
	msg.className = "message";
	if(pUser != "")
	{
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
	content.innerHTML = textProcess(pText);
	msg.appendChild(content);
	document.getElementById("container").appendChild(msg);
	window.scrollTo(0,document.body.scrollHeight);
}

function linkBuild(text)
{
	return text.replace(/(http:\/\/[^\s]+)/g, "<a href='$1' target='_blank'>$1</a>").replace(/(https:\/\/[^\s]+)/g, "<a href='$1' target='_blank'>$1</a>");
}

function textProcess(text)
{
	text = linkBuild(text);
	return text;
}