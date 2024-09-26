﻿import Matcher from "./Matcher.ts";


class MatcherShell {

    private Matchers: Matcher[];

    constructor(matcher: Matcher[]) {
        this.Matchers = matcher;
    }

    render(): HTMLElement {
        let borderDiv = document.createElement("div");
        borderDiv.classList.add("border");
        borderDiv.classList.add("border-3");
        borderDiv.classList.add("border-light-subtle");

        let header = this.renderMatcherHeader();
        let matcherTable = this.renderMatcherTable();
        let matcherButtons = this.renderMatcherButtons();

        borderDiv.appendChild(header);
        borderDiv.appendChild(matcherTable);
        borderDiv.appendChild(matcherButtons);

        return borderDiv;
    }

    private renderMatcherHeader(): HTMLElement {
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

    private renderMatcherTable(): HTMLElement {
        let table = document.createElement("table");
        table.classList.add("table");
        table.classList.add("table-bordered");
        table.classList.add("table-striped");

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
        table.appendChild(tableHead);

        let tableBody = this.renderMatcherTableBody();
        table.appendChild(tableBody);

        return table;
    }

    private renderMatcherTableBody() : HTMLElement {
        let body = document.createElement("tbody");
        this.Matchers.forEach((m) => {
            body.appendChild(m.render());
        });
        return body;
    }

    private renderMatcherButtons() : HTMLElement {
        let div = document.createElement("div");
        div.classList.add("d-flex");
        div.classList.add("justify-content-end");

        let removeButton = document.createElement("button");
        removeButton.classList.add("btn");
        removeButton.classList.add("btn-danger");
        removeButton.classList.add("me-2");
        removeButton.classList.add("mb-2");
        removeButton.innerHTML = "Remove All";
        div.appendChild(removeButton);

        let addButton = document.createElement("button");
        addButton.classList.add("btn");
        addButton.classList.add("btn-success");
        addButton.classList.add("me-2");
        addButton.classList.add("mb-2");
        addButton.innerHTML = "Add Matcher";
        div.appendChild(addButton);

        return div;
    }
}

export default MatcherShell;