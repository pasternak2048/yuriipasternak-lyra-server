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
 * Converts string into a slugified version
 */
window.lyra.slugify = function (input) {
    return input
        .trim()
        .toLowerCase()
        .replace(/[\s_]+/g, "-")
        .replace(/[^a-z0-9\-]/g, "")
        .replace(/-+/g, "-")
        .replace(/^-+|-+$/g, "");
};

/**
 * Reads from one input and writes the slugified version to another
 */
window.lyra.updateSlug = function (inputId, outputId) {
    const input = document.getElementById(inputId);
    const output = document.getElementById(outputId);
    if (input && output) {
        output.value = window.lyra.slugify(input.value);
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