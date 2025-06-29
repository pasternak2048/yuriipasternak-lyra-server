window.lyra = window.lyra || {};

/**
 * Generates a secure random base64 secret and writes it to input by ID
 */
window.lyra.generateSecret = function (inputId, byteLength = 32) {
    const random = crypto.getRandomValues(new Uint8Array(byteLength));
    const base64 = btoa(String.fromCharCode(...random));
    const input = document.getElementById(inputId);
    if (input) {
        input.value = base64;
    }
};

/**
 * Reads from one input and writes the slugified version to another
 */
window.lyra.updateSlug = function (inputId, outputId, prefix = "") {
    const input = document.getElementById(inputId);
    const output = document.getElementById(outputId);
    if (input && output) {
        output.value = window.lyra.slugify(input.value, prefix);
    }
};

/**
 * Filters the DisplayName input to allow only:
 * Latin letters, numbers, space, and . , - _ /
 */
window.lyra.filterDisplayName = function (input) {
    if (!input || !input.value) return;
    input.value = input.value.replace(/[^a-zA-Z0-9 .,\-_/]/g, '');
};

/**
 * Dynamically adds a new operation input to the list on the form.
 * - Generates a unique key (used for ASP.NET Core model binding).
 * - Appends a hidden input with name="Input.Operations.index" and that key.
 * - Appends a visible input for the operation value with name="Input.Operations[{key}]".
 * - Includes a "remove" button that allows deleting the input row.
 */
window.lyra.addOperation = function () {
    const key = crypto.randomUUID().replaceAll('-', '');
    const container = document.getElementById("operationList");

    const hidden = document.createElement("input");
    hidden.type = "hidden";
    hidden.name = "Input.Operations.index";
    hidden.value = key;

    const wrapper = document.createElement("div");
    wrapper.className = "input-group mb-2";
    wrapper.id = `op-${key}`;

    const input = document.createElement("input");
    input.type = "text";
    input.className = "form-control";
    input.name = `Input.Operations[${key}]`;

    const removeBtn = document.createElement("button");
    removeBtn.type = "button";
    removeBtn.className = "btn btn-outline-danger";
    removeBtn.innerHTML = `<i class="bi bi-x"></i>`;
    removeBtn.onclick = () => lyra.removeOperation(key);

    wrapper.appendChild(input);
    wrapper.appendChild(removeBtn);

    container.appendChild(hidden);
    container.appendChild(wrapper);
};

/**
 * Removes an operation input group from the form based on its unique key.
 * Also removes the corresponding hidden input used for model binding.
 * 
 * @param {string} key - The unique identifier for the operation input to remove.
 */
window.lyra.removeOperation = function (key) {
    const hiddenInput = document.querySelector(`input[name="Input.Operations.index"][value="${key}"]`);
    const wrapper = document.getElementById(`op-${key}`);
    if (hiddenInput) hiddenInput.remove();
    if (wrapper) wrapper.remove();
};