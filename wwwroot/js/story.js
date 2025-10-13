const scenes = scenes2; // Render this from Razor as JSON
let index = 0;

function showScene() {
  document.getElementById("scene").innerText = scenes[index].text;
}

document.getElementById("nextBtn").addEventListener("click", () => {
  if (index < scenes.length - 1) index++;
  showScene();
});

showScene();
