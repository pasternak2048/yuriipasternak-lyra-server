document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".secret-toggle").forEach(el => {
        el.addEventListener("click", () => {
            el.classList.toggle("reveal");
        });
    });
});