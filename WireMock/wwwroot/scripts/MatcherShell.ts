import Matcher from "./Matcher.js";
import {Section} from "./Section.js";

document.addEventListener('DOMContentLoaded', function () {
    const items = document.querySelectorAll('.mappingSection');
    items.forEach(item => {
        let data = item.getAttribute("rawData");
        let rawMap = JSON.parse(data!);
        
        createSection(item, "Path")
        createPath(item, rawMap);
        createSection(item, "Methods");
        createMethods(item, rawMap);
        createSection(item, "Headers");
        createHeaders(item, rawMap);
        createSection(item, "RAW");
    });
});

function createSection(item: Element, name : string) : void {
    let mockShells : MatcherShell[] = [];
    let section = new Section(name, mockShells);
    let headerDiv = section.render();
    item.appendChild(headerDiv);
}

function updateMethodsInRaw(event:Event, methods:any){
    // remove or add the method clicked from method array
    let target = event.target as HTMLInputElement;
    if (target.checked) {
        methods.push(target.name);
    } else {
        let index = methods.indexOf(target.name);
        if (index >= 0) {
            methods.splice(index, 1);
        }
    }
    // update the raw json mapping
    let rawMap = document.getElementById("rawMap-" + target.attributes.getNamedItem("mapGuid")?.value);
    if (rawMap?.textContent) {
        let rawMapContent = JSON.parse(rawMap.textContent);

        rawMapContent.Request.Methods = methods;
        rawMap.textContent = JSON.stringify(rawMapContent, null, 1);
    }
}

function createMethods(item: Element, rawMap : any) : void {
    let methods = rawMap.Request.Methods;
    let element = document.createElement("div");
    element.classList.add("d-flex");
    element.classList.add("align-items-center");
    element.classList.add("justify-content-center");
    
    let buttonGroup = document.createElement("div");
    buttonGroup.classList.add("btn-group");
    buttonGroup.setAttribute("role", "group");
    
    // GET
    let getInput = document.createElement("input");
    getInput.classList.add("btn-check");
    getInput.setAttribute("type", "checkbox");
    getInput.setAttribute("autocomplete", "off");
    getInput.setAttribute("name", "GET");
    getInput.setAttribute("mapGuid", rawMap.Guid);
    getInput.id = "btnCheck1";
    getInput.addEventListener("change", (event) => updateMethodsInRaw(event, methods));
    
    let getLabel = document.createElement("label");
    getLabel.classList.add("btn");
    getLabel.classList.add("btn-outline-primary");
    getLabel.setAttribute("for", "btnCheck1");
    getLabel.innerText = "GET";
    if (methods.indexOf("GET") >= 0) {
        getInput.checked = true;
    }
    buttonGroup.appendChild(getInput);
    buttonGroup.appendChild(getLabel);
    
    // POST
    let postInput = document.createElement("input");
    postInput.classList.add("btn-check");
    postInput.setAttribute("type", "checkbox");
    postInput.setAttribute("autocomplete", "off");
    postInput.setAttribute("name", "POST");
    postInput.setAttribute("mapGuid", rawMap.Guid);
    postInput.id = "btnCheck2";
    postInput.addEventListener("change", (event) => updateMethodsInRaw(event, methods));
    
    let postLabel = document.createElement("label");
    postLabel.classList.add("btn");
    postLabel.classList.add("btn-outline-primary");
    postLabel.setAttribute("for", "btnCheck2");
    postLabel.innerText = "POST";
    if (methods.indexOf("POST") >= 0) {
        postInput.checked = true;
    }
    buttonGroup.appendChild(postInput);
    buttonGroup.appendChild(postLabel);
    
    // PUT
    let putInput = document.createElement("input");
    putInput.classList.add("btn-check");
    putInput.setAttribute("type", "checkbox");
    putInput.setAttribute("autocomplete", "off");
    putInput.setAttribute("name", "PUT");
    putInput.setAttribute("mapGuid", rawMap.Guid);
    putInput.id = "btnCheck3";
    putInput.addEventListener("change", (event) => updateMethodsInRaw(event, methods));
    
    let putLabel = document.createElement("label");
    putLabel.classList.add("btn");
    putLabel.classList.add("btn-outline-primary");
    putLabel.setAttribute("for", "btnCheck3");
    putLabel.innerText = "PUT";
    if (methods.indexOf("PUT") >= 0) {
        putInput.checked = true;
    }
    buttonGroup.appendChild(putInput);
    buttonGroup.appendChild(putLabel);
    
    // DELETE
    let deleteInput = document.createElement("input");
    deleteInput.classList.add("btn-check");
    deleteInput.setAttribute("type", "checkbox");
    deleteInput.setAttribute("autocomplete", "off");
    deleteInput.id = "btnCheck4";
    deleteInput.setAttribute("name", "DELETE");
    deleteInput.setAttribute("mapGuid", rawMap.Guid);
    deleteInput.addEventListener("change", (event) => updateMethodsInRaw(event, methods));
    
    let deleteLabel = document.createElement("label");
    deleteLabel.classList.add("btn");
    deleteLabel.classList.add("btn-outline-primary");
    deleteLabel.setAttribute("for", "btnCheck4");
    deleteLabel.innerText = "DELETE";
    if (methods.indexOf("DELETE") >= 0) {
        deleteInput.checked = true;
    }
    buttonGroup.appendChild(deleteInput);
    buttonGroup.appendChild(deleteLabel);
    
    // PATCH
    let patchInput = document.createElement("input");
    patchInput.classList.add("btn-check");
    patchInput.setAttribute("type", "checkbox");
    patchInput.setAttribute("autocomplete", "off");
    patchInput.id = "btnCheck5";
    patchInput.setAttribute("name", "PATCH");
    patchInput.setAttribute("mapGuid", rawMap.Guid);
    patchInput.addEventListener("change", (event) => updateMethodsInRaw(event, methods));
    
    let patchLabel = document.createElement("label");
    patchLabel.classList.add("btn");
    patchLabel.classList.add("btn-outline-primary");
    patchLabel.setAttribute("for", "btnCheck5");
    patchLabel.innerText = "PATCH";
    if (methods.indexOf("PATCH") >= 0) {
        patchInput.checked = true;
    }
    buttonGroup.appendChild(patchInput);
    buttonGroup.appendChild(patchLabel);
    
    element.appendChild(buttonGroup);
    item.appendChild(element)
}

function createPath(item: Element, rawMap : any) : void {
    let paths = rawMap.Request.Path.Methods;
    let element = document.createElement("div");
    element.innerHTML = paths;
    item.appendChild(element)
}

function createHeaders(item: Element, rawMap : any) : void {
    let headers = rawMap.Request.Headers;
    for (let headerIndex = 0; headerIndex  < headers.length; headerIndex++) {
        let name = headers[headerIndex].Name;
        let ignoreCase = headers[headerIndex].IgnoreCase;
        let rawMatchers = headers[headerIndex].Matchers;

        let matchers: Matcher[] = [];
        rawMatchers.forEach((rawMatcher: { Guid: any; Id: any; Pattern: any; IgnoreCase: any; }) => {
            let matcher : Matcher = new Matcher({
                mappingGuid: rawMap.Guid,
                id: rawMatcher.Id,
                pattern: rawMatcher.Pattern,
                ignoreCase: rawMatcher.IgnoreCase,
                onClick: (id: string) => {
                }
            });
            matchers.push(matcher);
        });
        let matcherShell: MatcherShell = new MatcherShell(name, ignoreCase, matchers);
        console.log("render headerIndex: " + headerIndex);
        item.appendChild(matcherShell.render(headerIndex))
    }
}


export default class MatcherShell {

    private Matchers: Matcher[];
    private name: string;
    private ignoreCase: boolean = false;

    constructor(name: string, ignoreCase: boolean, matcher: Matcher[]) {
        this.Matchers = matcher;
        this.name = name;
        this.ignoreCase = ignoreCase;
    }

    render(headerIndex : number): HTMLElement {
        let borderDiv = document.createElement("div");
        borderDiv.classList.add("border");
        borderDiv.classList.add("border-3");
        borderDiv.classList.add("border-light-subtle");

        let header = this.renderMatcherHeader();
        let matcherTable = this.renderMatcherTable(headerIndex);
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
        headerNameInput.value = this.name;
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

    private renderMatcherTable(headerIndex: number): HTMLElement {
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

        let tableBody = this.renderMatcherTableBody(headerIndex);
        table.appendChild(tableBody);

        return table;
    }

    private renderMatcherTableBody(headerIndex : number): HTMLElement {
        let body = document.createElement("tbody");
        
        for(let i = 0; i < this.Matchers.length; i++){
            body.appendChild(this.Matchers[i].render(headerIndex, i));
        }
        return body;
    }

    private renderMatcherButtons(): HTMLElement {
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