import generateRandomId from "./helper/generateRandomId.js"


export interface MatcherProps {
    MappingGuid: string;
    Id: string;
    pattern: string;
    ignoreCase: boolean;
    onClick: (id: string, mappingGuid: string) => void;
}

export default class Matcher {

    public MappingGuid: string;
    public Id: string;
    private pattern: string;
    private ignoreCase: boolean;
    private onRemoveClick: (id: string, matchingGuid: string) => void;

    constructor(props: MatcherProps) {
        this.MappingGuid = props.MappingGuid;
        this.Id = props.Id;
        this.pattern = props.pattern;
        this.ignoreCase = props.ignoreCase;
        this.onRemoveClick = props.onClick;
    };
    
    renderPathMatcher(matcherIndex : number) : HTMLElement {

        let row = document.createElement("tr");
        row.setAttribute("id", "matcher-" + this.Id);
        row.appendChild(this.renderMatcherName());
        row.appendChild(this.renderPathPatternCell(matcherIndex))
        
        let rawMap = document.getElementById("rawMap-" + this.MappingGuid);
        if (rawMap?.textContent) {
            row.appendChild(this.renderPathIgnoreCaseCell(matcherIndex));
            
        }
            
        row.appendChild(this.renderRemoveButton());
        return row;
    }


    renderHeaderMatcher(headerIndex: number, matcherIndex : number): HTMLElement {

        
        let row = document.createElement("tr");
        row.setAttribute("id", "matcher-" + this.Id);
        row.appendChild(this.renderMatcherName());
        row.appendChild(this.renderHeaderPatternCell(headerIndex, matcherIndex));

        let rawMap = document.getElementById("rawMap-" + this.MappingGuid);
        if (rawMap?.textContent) {
            row.appendChild(this.renderHeaderIgnoreCaseCell(headerIndex, matcherIndex));
        }

        row.appendChild(this.renderRemoveButton());

        return row;
    }

    private renderMatcherName(): HTMLElement {
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

    private renderPathPatternCell(matcherIndex : number): HTMLElement {
        let id = "pattern-" + generateRandomId(10);
        let patternCell = document.createElement("td");

        let patternLabel = document.createElement("label");
        patternLabel.setAttribute("for", id);

        let patternInput = document.createElement("input");
        patternInput.setAttribute("id", id);
        patternInput.setAttribute("placeholder", "Pattern");
        patternInput.value = this.pattern;
        patternInput.addEventListener("input", () => {
            let rawMap = document.getElementById("rawMap-" + this.MappingGuid);
            if (rawMap?.textContent) {
                let rawMapContent = JSON.parse(rawMap.textContent);

                rawMapContent.Request.Path.Matchers[matcherIndex].Pattern = patternInput.value;
                rawMap.textContent = JSON.stringify(rawMapContent, null, 1);
            }
        });

        patternCell.appendChild(patternLabel);
        patternCell.appendChild(patternInput);

        return patternCell;
    }
    
    private renderHeaderPatternCell(headerIndex: number, matcherIndex : number): HTMLElement {
        let id = "pattern-" + generateRandomId(10);
        let patternCell = document.createElement("td");

        let patternLabel = document.createElement("label");
        patternLabel.setAttribute("for", id);

        let patternInput = document.createElement("input");
        patternInput.setAttribute("id", id);
        patternInput.setAttribute("placeholder", "Pattern");
        patternInput.value = this.pattern;
        patternInput.addEventListener("input", () => {
            let rawMap = document.getElementById("rawMap-" + this.MappingGuid);
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

    private renderPathIgnoreCaseCell(matcherIndex : number): HTMLElement {
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
        checkInput.addEventListener("change", () => {
            let rawMap = document.getElementById("rawMap-" + this.MappingGuid);
            if (rawMap?.textContent) {
                let rawMapContent = JSON.parse(rawMap.textContent);
                rawMapContent.Request.Path.Matchers[matcherIndex].IgnoreCase = checkInput.checked;
                rawMap.textContent = JSON.stringify(rawMapContent, null, 1);
            }
        });

        let checkLabel = document.createElement("label");
        checkLabel.classList.add("form-check-label");
        checkLabel.setAttribute("for", id);
        checkLabel.innerText = "Ignore Case"

        checkDiv.appendChild(checkInput)
        checkDiv.appendChild(checkLabel);
        ignoreCaseCell.appendChild(checkDiv);
        return ignoreCaseCell;
    }

    private renderHeaderIgnoreCaseCell(headerIndex: number, matcherIndex : number): HTMLElement {
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
        checkInput.addEventListener("change", () => {
            let rawMap = document.getElementById("rawMap-" + this.MappingGuid);
            if (rawMap?.textContent) {
                let rawMapContent = JSON.parse(rawMap.textContent);
                rawMapContent.Request.Headers[headerIndex].Matchers[matcherIndex].IgnoreCase = checkInput.checked;
                rawMap.textContent = JSON.stringify(rawMapContent, null, 1);
            }
        });

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
        button.addEventListener("click", () => this.onRemoveClick(this.Id, this.MappingGuid));
        buttonCell.appendChild(button);
        return buttonCell;
    }
}