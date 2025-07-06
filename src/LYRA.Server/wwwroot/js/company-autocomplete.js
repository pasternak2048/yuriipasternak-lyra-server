// Initializes company autocomplete functionality for the specified input group
function initCompanyAutocomplete({
    inputId = "companyInput",
    hiddenInputId = "companyIdInput",
    dropdownId = "companyDropdown",
    fetchUrl = "/Shared/CompaniesAutocomplete"
}) {
    const input = document.getElementById(inputId);
    const hiddenInput = document.getElementById(hiddenInputId);
    const dropdown = document.getElementById(dropdownId);
    let debounceTimer;

    if (!input || !hiddenInput || !dropdown) return;

    input.addEventListener("input", function () {
        const query = this.value;

        if (query.trim() === "") {
            hiddenInput.value = "";
        }

        if (query.length < 2) {
            dropdown.innerHTML = "";
            dropdown.classList.remove("show");
            return;
        }

        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => {
            fetch(`${fetchUrl}?term=${encodeURIComponent(query)}`)
                .then(res => res.json())
                .then(data => {
                    dropdown.innerHTML = "";

                    if (!data.length) {
                        dropdown.innerHTML = '<div class="dropdown-item disabled">No results</div>';
                        dropdown.classList.add("show");
                        return;
                    }

                    data.forEach(c => {
                        const label = `${c.systemName} (${c.displayName})`;
                        const item = document.createElement("div");
                        item.className = "dropdown-item";
                        item.textContent = label;
                        item.dataset.id = c.id;

                        item.onclick = () => {
                            input.value = label;
                            hiddenInput.value = c.id;
                            dropdown.innerHTML = "";
                            dropdown.classList.remove("show");
                            input.blur();
                        };

                        dropdown.appendChild(item);
                    });

                    dropdown.classList.add("show");
                });
        }, 300);
    });

    input.addEventListener("blur", () => {
        setTimeout(() => {
            dropdown.innerHTML = "";
            dropdown.classList.remove("show");
        }, 200);
    });
}