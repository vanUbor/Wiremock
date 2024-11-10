import generateRandomId from "./helper/generateRandomId.js"


export interface MatcherProps {
    mappingGuid: string;
    id: string;
    pattern: string;
    ignoreCase: boolean;
    onClick: (id: string) => void;
}

export default class Matcher {

    private mappingGuid: string;
    private id: string;
    private pattern: string;
    private ignoreCase: boolean;
    private onRemoveClick: (id: string) => void;

    constructor(props: MatcherProps) {
        this.mappingGuid = props.mappingGuid;
        this.id = props.id;
        this.pattern = props.pattern;
        this.ignoreCase = props.ignoreCase;
        this.onRemoveClick = props.onClick;
    };


    render(headerIndex: number, matcherIndex : number): HTMLElement {
        let row = document.createElement("tr");
        row.setAttribute("id", this.id);

        row.appendChild(this.renderNameCell());
        row.appendChild(this.renderPatternCell(headerIndex, matcherIndex));
        row.appendChild(this.renderIgnoreCaseCell());
        row.appendChild(this.renderRemoveButton());

        return row;
    }

    private renderNameCell(): HTMLElement {
        let matcherColumn = document.createElement("td");

        let dropdownDiv = document.createElement("div");
        dropdownDiv.classList.add("dropdown");

        let button = document.createElement("button");
        button.classList.add("btn");
        button.classList.add("btn-secondary");
        button.classList.add("dropdown-toggle");
        button.type = "button";
        button.setAttribute("data-bs-toggle", "dropdown");
        button.setAttribute("aria-expanded", "false");
        button.innerText = "WildCardMatcher"

        let list = document.createElement("ul");
        list.classList.add("dropdown-menu");
        let wildCardListItem = document.createElement("li");
        let wildcardMatcherLink = document.createElement("a");
        wildcardMatcherLink.classList.add("dropdown-item");
        wildcardMatcherLink.href = "#"
        wildcardMatcherLink.innerText = "WildCardMatcher"
        wildCardListItem.appendChild(wildcardMatcherLink);

        let regExListItem = document.createElement("li");
        let regExLink = document.createElement("a");
        regExLink.classList.add("dropdown-item");
        regExLink.href = "#"
        regExLink.innerText = "RegExMatcher"
        regExListItem.appendChild(regExLink);

        list.appendChild(wildCardListItem);
        list.appendChild(regExListItem);

        dropdownDiv.appendChild(list);
        dropdownDiv.appendChild(button);
        dropdownDiv.appendChild(list);

        matcherColumn.appendChild(dropdownDiv);
        return matcherColumn;
    }

    private renderPatternCell(headerIndex: number, matcherIndex : number): HTMLElement {
        let id = generateRandomId(10);
        let patternCell = document.createElement("td");

        let patternLabel = document.createElement("label");
        patternLabel.setAttribute("for", id);

        let patternInput = document.createElement("input");
        patternInput.setAttribute("id", id);
        patternInput.setAttribute("placeholder", "Pattern");
        patternInput.value = this.pattern;
        patternInput.addEventListener("input", () => {
            let rawMap = document.getElementById("rawMap-" + this.mappingGuid);
            if (rawMap?.textContent) {
                let rawMapContent = JSON.parse(rawMap.textContent);
                
                rawMapContent.Request.Headers[headerIndex].Matchers[matcherIndex].Pattern = patternInput.value;
                rawMap.textContent = JSON.stringify(rawMapContent, null, 1);
            }
        });

        patternCell.appendChild(patternLabel);
        patternCell.appendChild(patternInput);

        return patternCell;
    }

    private renderIgnoreCaseCell(): HTMLElement {
        let id = generateRandomId(10);

        let ignoreCaseCell = document.createElement("td");
        let checkDiv = document.createElement("div");
        checkDiv.classList.add("form-check");
        checkDiv.classList.add("form-switch");

        let checkInput = document.createElement("input");
        checkInput.classList.add("form-check-input");
        checkInput.setAttribute("type", "checkbox");
        checkInput.setAttribute("role", "switch");
        checkInput.setAttribute("id", id);
        checkInput.checked = this.ignoreCase;

        let checkLabel = document.createElement("label");
        checkLabel.classList.add("form-check-label");
        checkLabel.setAttribute("for", id);
        checkLabel.innerText = "Ignore Case"

        checkDiv.appendChild(checkInput)
        checkDiv.appendChild(checkLabel);
        ignoreCaseCell.appendChild(checkDiv);
        return ignoreCaseCell;
    }

    private renderRemoveButton(): HTMLElement {
        let buttonCell = document.createElement("td");
        let button = document.createElement("button");
        button.innerHTML = "X";
        button.classList.add("btn");
        button.classList.add("btn-danger");
        button.addEventListener("click", () => this.onRemoveClick(this.id));
        buttonCell.appendChild(button);
        return buttonCell;
    }
}