
document.addEventListener('DOMContentLoaded', function () {
  
  const sidebar = document.querySelector('.story-list, .sidebar');
  const editor  = document.getElementById('editor-surface');
  if (!sidebar || !editor) return;

  
  sidebar.addEventListener('click', async function (e) {
    const link = e.target.closest('a.story-item');
    if (!link) return;

    e.preventDefault();

    
    sidebar.querySelectorAll('.story-item.is-active')
      .forEach(a => a.classList.remove('is-active'));
    link.classList.add('is-active');

    
    const id = link.dataset.id || link.getAttribute('href').split('/').pop();

    try {
      
      const res = await fetch(`/Story/EditorPartial?id=${encodeURIComponent(id)}`, {
        headers: { 'X-Requested-With': 'fetch' }
      });

      if (!res.ok) throw new Error('Load failed: ' + res.status);

      const html = await res.text();

      
      editor.innerHTML = html;

      
      editor.dataset.storyId = id;

      console.log(`Loaded story ${id}`);
    } catch (err) {
      console.error(err);
      editor.innerHTML = `
        <div class="empty-state">
          <h3>Couldn’t load this story.</h3>
          <p>${err.message}</p>
        </div>`;
    }
  });
});


function workspaceModal() { return document.getElementById('workspace-modal'); }
function openModal(html) {
  const m = workspaceModal();
  if (!m) return;
  m.innerHTML = `<div class="modal-sheet">${html}</div>`;
  m.hidden = false;
}
function closeModal() {
  const m = workspaceModal();
  if (!m) return;
  m.hidden = true;
  m.innerHTML = '';
}


document.addEventListener('click', async (e) => {
  const btn = e.target.closest('.js-edit-scene');
  if (!btn) return;

  e.preventDefault();
  const id = btn.dataset.id;
  if (!id) return;

  const res = await fetch(`/Scene/Update/${encodeURIComponent(id)}`, {
    headers: { 'X-Requested-With': 'fetch' } 
  });

  if (!res.ok) {
    alert('Could not load edit form.');
    return;
  }

  const html = await res.text();
  openModal(html);
});


document.addEventListener('click', (e) => {
  if (e.target.closest('.js-close-modal')) {
    e.preventDefault();
    closeModal();
  }
});


document.addEventListener('submit', async (e) => {
  const form = e.target;
  if (!form.closest('#workspace-modal')) return; 

  e.preventDefault();

  try {
    const res = await fetch(form.action, {
      method: form.method || 'POST',
      body: new FormData(form),
      headers: { 'X-Requested-With': 'fetch' }
    });

    if (!res.ok) throw new Error('Save failed');

    
    closeModal();

    // refresh the right panel with the same story
    const editor = document.getElementById('editor-surface');
    const storyId = editor?.dataset.storyId;
    if (storyId) {
      const reload = await fetch(`/Story/EditorPartial?id=${encodeURIComponent(storyId)}`, {
        headers: { 'X-Requested-With': 'fetch' }
      });
      if (reload.ok) editor.innerHTML = await reload.text();
    }
  } catch (err) {
    alert(err.message || 'Something went wrong while saving.');
  }
});

