/// <summary>
/// Initializes an autocomplete dropdown for any entity input group.
/// </summary>
function initAutocomplete({
    inputId = "autocompleteInput",
    hiddenInputId = "autocompleteHiddenId",
    dropdownId = "autocompleteDropdown",
    fetchUrl = "",
    getLabel = (item) => `${item.systemName} (${item.displayName})`
}) {
    const input = document.getElementById(inputId);
    const hiddenInput = document.getElementById(hiddenInputId);
    const dropdown = document.getElementById(dropdownId);
    let debounceTimer;

    if (!input || !hiddenInput || !dropdown || !fetchUrl) return;

    input.addEventListener("input", function () {
        const query = this.value;

        // Clear selection if input manually cleared
        if (query.trim() === "") {
            hiddenInput.value = "";
        }

        // Skip search if too short
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

                    data.forEach(itemData => {
                        const label = getLabel(itemData);
                        const item = document.createElement("div");
                        item.className = "dropdown-item";
                        item.textContent = label;
                        item.dataset.id = itemData.id;

                        item.onclick = () => {
                            input.value = label;
                            hiddenInput.value = itemData.id;
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