document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".show-more").forEach(btn => {
        btn.addEventListener("click", function () {
            let descContent = this.previousElementSibling;
            descContent.classList.toggle("expanded");
            this.textContent = descContent.classList.contains("expanded") ? "Show less" : "Show more";
        });
    });
});