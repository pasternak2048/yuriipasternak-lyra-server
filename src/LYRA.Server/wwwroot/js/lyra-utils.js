window.lyra = (() => {
    function addOperation() {
        const list = document.getElementById("operationList");
        const key = crypto.randomUUID().replace(/-/g, "");

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
        input.placeholder = "GET /api/orders or POST /api/orders/*";

        const button = document.createElement("button");
        button.type = "button";
        button.className = "btn btn-outline-danger";
        button.innerHTML = '<i class="bi bi-x"></i>';
        button.onclick = () => lyra.removeOperation(key);

        wrapper.appendChild(input);
        wrapper.appendChild(button);

        list.appendChild(hidden);
        list.appendChild(wrapper);
    }

    function removeOperation(key) {
        document.getElementById(`op-${key}`)?.remove();

        const hiddenInputs = document.querySelectorAll('input[name="Input.Operations.index"]');
        for (const input of hiddenInputs) {
            if (input.value === key) {
                input.remove();
                break;
            }
        }
    }

    return {
        addOperation,
        removeOperation
    };
})();