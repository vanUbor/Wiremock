import Matcher from "./Matcher.js";
import generateRandomId from "./helper/generateRandomId.js";


export default class MatcherShell {

    private Matchers: Matcher[];
    private readonly name: string;
    private readonly ignoreCase: boolean = false;
    private readonly mappingGuid : string;

    constructor(mappingGuid :string, name: string, ignoreCase: boolean, matcher: Matcher[]) {
        this.Matchers = matcher;
        this.name = name;
        this.ignoreCase = ignoreCase;
        this.mappingGuid = mappingGuid;
    }

    renderPaths(): HTMLTableElement {
        let table = this.renderMatcherTable();
        let body = this.renderPathMatcherTableBody()
        table.appendChild(body);
        return table;
    }
    
    renderHeaders(headerIndex: number,
                  onNameChanged: (newName: string) => void,
                  onIgnoreCaseChange: (ignoreCase: boolean) => void): HTMLElement {
        let borderDiv = document.createElement("div");
        borderDiv.classList.add("border");
        borderDiv.classList.add("border-3");
        borderDiv.classList.add("border-light-subtle");

        let header = this.renderMatcherHeader(onNameChanged, onIgnoreCaseChange);
        let matcherTable = this.renderMatcherTable();
        let tableBody = this.renderHeaderMatcherTableBody(headerIndex);
        matcherTable.appendChild(tableBody);
        let matcherButtons = this.renderMatcherButtons(tableBody);

        borderDiv.appendChild(header);
        borderDiv.appendChild(matcherTable);
        borderDiv.appendChild(matcherButtons);

        return borderDiv;
    }


    private renderMatcherHeader(
        onNameChanged: (newName: string) => void,
        onIgnoreCaseChange: (ignoreCase: boolean) => void): HTMLElement {
        let headerDiv = document.createElement("div");
        headerDiv.classList.add("d-flex");
        headerDiv.classList.add("justify-content-around");
        headerDiv.style.padding = "10px";

        // Name
        let headerNameDiv = document.createElement("div");
        headerNameDiv.classList.add("flex-column");

        // Name Label
        let headerLabel = document.createElement("label");
        headerLabel.setAttribute("for", "Name");
        headerLabel.innerHTML = "Name:&nbsp";
        headerNameDiv.appendChild(headerLabel);

        // Name Input
        let headerNameInput = document.createElement("input");
        headerNameInput.id = "Name";
        headerNameInput.type = "text";
        headerNameInput.value = this.name;
        headerNameInput.oninput = () => {
            onNameChanged(headerNameInput.value);
        };
        headerNameDiv.appendChild(headerNameInput);


        // Ignore Case
        let headerInputDiv = document.createElement("div");
        headerInputDiv.classList.add("form-check");
        headerInputDiv.classList.add("form-switch");

        // Ignore Case Input
        let headerInput = document.createElement("input");
        headerInput.classList.add("form-check-input");
        headerInput.setAttribute("type", "checkbox");
        headerInput.setAttribute("role", "switch");
        headerInput.setAttribute("id", "flexSwitchCheckDefault")
        headerInput.checked = this.ignoreCase;
        headerInput.onchange = () => {
            onIgnoreCaseChange(headerInput.checked);
        }
        headerInputDiv.appendChild(headerInput);

        // Ignore Case Label
        let headerIgnoreCaseLabel = document.createElement("label");
        headerIgnoreCaseLabel.classList.add("form-check-label");
        headerIgnoreCaseLabel.setAttribute("for", "flexSwitchCheckDefault")
        headerIgnoreCaseLabel.innerText = "IgnoreCase";
        headerInputDiv.appendChild(headerIgnoreCaseLabel);


        headerDiv.appendChild(headerNameDiv);
        headerDiv.appendChild(headerInputDiv);

        return headerDiv;
    }


    private renderMatcherTable(): HTMLTableElement {
        let table = document.createElement("table");
        table.classList.add("table");
        table.classList.add("table-bordered");
        table.classList.add("table-striped");

        let tableHead = this.renderMatcherTableHead();
        table.appendChild(tableHead);

        return table;
    }

    private renderMatcherTableHead(): HTMLElement {
        let tableHeaderRow = document.createElement("tr");

        let tableHeaderNameCell = document.createElement("th");
        tableHeaderNameCell.setAttribute("scope", "col");
        tableHeaderNameCell.innerHTML = "Name";
        tableHeaderRow.appendChild(tableHeaderNameCell);

        let tableHeaderPatternCell = document.createElement("th");
        tableHeaderPatternCell.setAttribute("scope", "col");
        tableHeaderPatternCell.innerHTML = "Pattern";
        tableHeaderRow.appendChild(tableHeaderPatternCell);

        let tableHeaderIgnoreCaseCell = document.createElement("th");
        tableHeaderIgnoreCaseCell.setAttribute("scope", "col");
        tableHeaderIgnoreCaseCell.innerHTML = "Ignore Case";
        tableHeaderRow.appendChild(tableHeaderIgnoreCaseCell);

        let tableHeaderEmptyCell = document.createElement("th");
        tableHeaderEmptyCell.setAttribute("scope", "col");
        tableHeaderEmptyCell.innerHTML = "";
        tableHeaderRow.appendChild(tableHeaderEmptyCell);

        let tableHead = document.createElement("thead");
        tableHead.appendChild(tableHeaderRow);

        return tableHead;
    }

    private renderHeaderMatcherTableBody(headerIndex: number): HTMLTableSectionElement {
        let body = document.createElement("tbody");

        for (let i = 0; i < this.Matchers.length; i++) {
            body.appendChild(this.Matchers[i].renderHeaderMatcher(headerIndex, i));
        }
        return body;
    }

    renderPathMatcherTableBody(): HTMLTableSectionElement {
        let body = document.createElement("tbody");
        for (let i = 0; i < this.Matchers.length; i++) {
            body.appendChild(this.Matchers[i].renderPathMatcher(i));
        }
        return body;
    }
    
    addPathMatcher(body: HTMLTableSectionElement, matcher: Matcher): void {
        this.Matchers.push(matcher);
        body.appendChild(matcher.renderPathMatcher(this.Matchers.length));
    }

    // Creates the delete button at the end of every matcher row
    renderMatcherButtons(tbody: HTMLTableSectionElement): HTMLElement {
        let div = document.createElement("div");
        div.classList.add("d-flex");
        div.classList.add("justify-content-end");
        this.renderAddMatcherButton(div, tbody);
        return div;
    }

    renderAddMatcherButton(div: HTMLDivElement, tbody: HTMLTableSectionElement) {
        let addButton = document.createElement("button");
        addButton.classList.add("btn");
        addButton.classList.add("btn-success");
        addButton.classList.add("me-2");
        addButton.classList.add("mb-2");
        addButton.innerHTML = "Add Matcher";
        addButton.addEventListener("click", () => {
           let matcher = Matcher.createMatcher(this.mappingGuid, 
               { Id: generateRandomId(10), Pattern: "", IgnoreCase: false});
           this.addPathMatcher(tbody, matcher);
        })
        tbody.appendChild(addButton);
        div.appendChild(addButton);
    }
}