

setTimeout(function(){ 
    var img = document.createElement("img");
img.src = "https://media.giphy.com/media/LRVnPYqM8DLag/giphy.gif";
var src = document.getElementById("swagger-ui");
var x = document.createElement("H1");
  var t = document.createTextNode("Alguna pregunta?");
  x.appendChild(t);
  document.body.appendChild(x);
  document.body.appendChild(img);
 }, 3000);
