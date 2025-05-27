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