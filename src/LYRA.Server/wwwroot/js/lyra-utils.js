window.lyra = (() => {
    function createMethodSelect(key) {
        const select = document.createElement("select");
        select.className = "form-select";
        select.name = `Input.Rules[${key}].Method`;
        select.style.maxWidth = "140px";

        const methods = ["ANY", "GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS"];

        for (const method of methods) {
            const option = document.createElement("option");
            option.value = method;
            option.textContent = method;

            if (method === "GET") {
                option.selected = true;
            }

            select.appendChild(option);
        }

        return select;
    }

    function addRouteRule() {
        const list = document.getElementById("routeRuleList");
        if (!list) return;

        const key = crypto.randomUUID().replace(/-/g, "");

        const hidden = document.createElement("input");
        hidden.type = "hidden";
        hidden.name = "Input.Rules.index";
        hidden.value = key;

        const wrapper = document.createElement("div");
        wrapper.className = "input-group mb-2";
        wrapper.id = `rule-${key}`;

        const methodSelect = createMethodSelect(key);

        const pathInput = document.createElement("input");
        pathInput.type = "text";
        pathInput.className = "form-control";
        pathInput.name = `Input.Rules[${key}].PathPattern`;
        pathInput.placeholder = "/api/orders/* or /*";

        const button = document.createElement("button");
        button.type = "button";
        button.className = "btn btn-outline-danger";
        button.innerHTML = '<i class="bi bi-x"></i>';
        button.onclick = () => lyra.removeRouteRule(key);

        wrapper.appendChild(methodSelect);
        wrapper.appendChild(pathInput);
        wrapper.appendChild(button);

        list.appendChild(hidden);
        list.appendChild(wrapper);
    }

    function removeRouteRule(key) {
        document.getElementById(`rule-${key}`)?.remove();

        const hiddenInputs = document.querySelectorAll('input[name="Input.Rules.index"]');
        for (const input of hiddenInputs) {
            if (input.value === key) {
                input.remove();
                break;
            }
        }
    }

    return {
        addRouteRule,
        removeRouteRule
    };
})();