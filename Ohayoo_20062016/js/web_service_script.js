console.info("Detect WebService");

var elem = document.createElement("div");
elem.id = '_version-add-on';
elem.setAttribute("data-last-updated-time", last_updated_time);
elem.setAttribute("style", "display: none;");
elem.innerHTML = version_tool;
document.body.insertBefore(elem,document.body.childNodes[0]);