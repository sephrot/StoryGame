document.addEventListener("DOMContentLoaded", () => {
  const host = document.getElementById("scene-tree");
  if (!host) return;

  const storyId = host.getAttribute("data-story-id");
  host.classList.add("scene-tree");

  // try path style first, then query string as fallback
  const urls = [
    `/Story/TreeData/${encodeURIComponent(storyId)}`,
    `/Story/TreeData?id=${encodeURIComponent(storyId)}`
  ];

  attempt(urls, 0);

  function attempt(list, i) {
    if (i >= list.length) {
      host.textContent = "Could not load scene map";
      return;
    }
    const url = list[i];
    console.log("SceneTree -> GET", url);
    fetch(url, { headers: { "Accept": "application/json" } })
      .then(async r => {
        if (!r.ok) {
          const t = await r.text().catch(() => "");
          throw new Error(`HTTP ${r.status} ${r.statusText} — ${t}`);
        }
        return r.json();
      })
      .then(payload => {
        if (!payload || !Array.isArray(payload.nodes)) {
          throw new Error("Bad JSON shape (expected { nodes: [...] })");
        }
        const tree = buildTree(payload.nodes);
        host.innerHTML = "";
        host.appendChild(renderTree(tree));
      })
      .catch(err => {
        console.warn("Scene tree fetch failed:", err);
        attempt(list, i + 1); // try next URL pattern
      });
  }

  function buildTree(rows) {
    const byId = new Map(rows.map(r => [r.id, { ...r, children: [] }]));
    const roots = [];
    for (const node of byId.values()) {
      if (node.parentId == null) roots.push(node);
      else (byId.get(node.parentId)?.children ?? roots).push(node);
    }
    const sortRec = n => { n.children.sort((a,b)=>(a.title||"").localeCompare(b.title||"")); n.children.forEach(sortRec); };
    roots.sort((a,b)=>(a.title||"").localeCompare(b.title||"")); roots.forEach(sortRec);
    return roots;
  }

  function renderTree(nodes) {
    const ul = document.createElement("ul");
    for (const n of nodes) {
      const li = document.createElement("li");

      const a = document.createElement("a");
      a.className = "scene-node";
      a.href = `/Story/Details/${n.id}`;
      a.textContent = n.title || `Scene ${n.id}`;

      const tag = document.createElement("div");
      tag.className = "scene-tag";
      tag.textContent = n.parentId == null ? "root" : (n.children?.length ? "branch" : "leaf");
      a.appendChild(document.createElement("br"));
      a.appendChild(tag);

      li.appendChild(a);
      if (n.children?.length) li.appendChild(renderTree(n.children));
      ul.appendChild(li);
    }
    return ul;
  }
});
