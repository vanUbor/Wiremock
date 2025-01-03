import {Section} from "./Section.js";
import Matcher from "./Matcher.js";
import MatcherShell from "./MatcherShell.js";
import generateRandomId from "./helper/generateRandomId.js";

document.addEventListener('DOMContentLoaded', function () {
    const items = document.querySelectorAll('.mappingSection');
    items.forEach(item => {
        let data = item.getAttribute("rawData");
        let rawMap = JSON.parse(data!);

        createMethods(item, rawMap);
        item.appendChild(document.createElement("br"));

        createPaths(item, rawMap);
        item.appendChild(document.createElement("br"));

        createHeaders(item, rawMap);
        item.appendChild(document.createElement("br"));

        createSection(item, "RAW");
    });
});

function createSection(item: Element, name: string): HTMLElement {
    let mockShells: MatcherShell[] = [];
    let section = new Section(name, mockShells);
    return section.render();
}

function updateMethodsInRaw(event: Event, methods: any) {
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

function createPaths(item: Element, rawMap: any): void {

    let pathMatchers = rawMap.Request.Path.Matchers;
    let matchers: Matcher[] = [];
    pathMatchers.forEach((rawMatcher: { Guid: any; Id: any; Pattern: any; IgnoreCase: any; }) => {
        let id = generateRandomId(10);
        let matcher: Matcher = new Matcher({
            MappingGuid: rawMap.Guid,
            Id: id,
            pattern: rawMatcher.Pattern,
            ignoreCase: rawMatcher.IgnoreCase,
            // removes the matcher from the raw mapping json
            onClick: (id: string, mappingGuid: string) => {
                let rawMap = document.getElementById("rawMap-" + mappingGuid);
                if (rawMap?.textContent) {
                    let rawMapContent = JSON.parse(rawMap.textContent);
                    rawMapContent.Request.Path.Matchers =
                        rawMapContent.Request.Path.Matchers.filter((m: { Guid: any; Pattern: any; IgnoreCase: any; }) =>
                            m.Pattern != rawMatcher.Pattern);
                    rawMap.textContent = JSON.stringify(rawMapContent, null, 1);
                }
                document.getElementById("matcher-" + id)?.remove();
            }
        });
        matchers.push(matcher);
    });

    let matcherShell = new MatcherShell("", false, matchers);
    let pathTable = matcherShell.renderPaths();

    let boarder = document.createElement("div");
    boarder.classList.add("border");
    boarder.classList.add("border-3");
    boarder.classList.add("border-light-subtle");
    let headerDiv = createSection(item, "Paths");
    boarder.appendChild(headerDiv);
    boarder.appendChild(pathTable);
    let matcherButtons = matcherShell.renderMatcherButtons();
    boarder.appendChild(matcherButtons);
    item.appendChild(boarder);
}

function createMethods(item: Element, rawMap: any): void {
    let headerDiv = createSection(item, "Methods");

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

    let boarder = document.createElement("div");
    boarder.classList.add("border");
    boarder.classList.add("border-3");
    boarder.classList.add("border-light-subtle");
    boarder.appendChild(headerDiv);
    boarder.appendChild(element);

    item.appendChild(boarder);
}

function createHeaders(item: Element, rawMap: any): void {
    let headerDiv = createSection(item, "Headers");
    let boarder = document.createElement("div");
    boarder.classList.add("border");
    boarder.classList.add("border-3");
    boarder.classList.add("border-light-subtle");
    boarder.appendChild(headerDiv);

    let headers = rawMap.Request.Headers;
    for (let headerIndex = 0; headerIndex < headers.length; headerIndex++) {
        let name = headers[headerIndex].Name;
        let ignoreCase = headers[headerIndex].IgnoreCase;
        let headerMatchers = headers[headerIndex].Matchers;

        let matchers: Matcher[] = [];
        headerMatchers.forEach((rawMatcher: { Guid: any; Id: any; Pattern: any; IgnoreCase: any; }) => {
            let id = generateRandomId(10);
            let matcher: Matcher = new Matcher({
                MappingGuid: rawMap.Guid,
                Id: id,
                pattern: rawMatcher.Pattern,
                ignoreCase: rawMatcher.IgnoreCase,
                onClick: (id: string, mappingGuid: string) => {
                    let rawMap = document.getElementById("rawMap-" + mappingGuid);
                    if (rawMap?.textContent) {
                        let rawMapContent = JSON.parse(rawMap.textContent);
                        rawMapContent.Request.Headers[headerIndex].Matchers =
                            rawMapContent.Request.Headers[headerIndex].Matchers.filter((m: {
                                Guid: any;
                                Pattern: any;
                                IgnoreCase: any;
                            }) =>
                                m.Pattern != rawMatcher.Pattern);
                        rawMap.textContent = JSON.stringify(rawMapContent, null, 1);
                    }
                    document.getElementById("matcher-" + id)?.remove();
                }
            });
            matchers.push(matcher);
        });
        let matcherShell: MatcherShell = new MatcherShell(name, ignoreCase, matchers);

        boarder.appendChild(matcherShell.renderHeaders(headerIndex, (newName) => {
            let rm = document.getElementById("rawMap-" + rawMap.Guid);
            if (rm?.textContent) {
                let rawMapContent = JSON.parse(rm.textContent);
                rawMapContent.Request.Headers[headerIndex].Name = newName;
                rm.textContent = JSON.stringify(rawMapContent, null, 1);
            }
        }));
    }
    item.appendChild(boarder);
}