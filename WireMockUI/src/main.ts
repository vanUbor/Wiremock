import Matcher from "./components/Matcher";
import generateRandomId from "./helper/generateRandomId"


function removeMatcher(id: string): void {
    console.log(id);
    const matcherTableElement = document.getElementById('matcherTableBody');
    const matcherRow = document.getElementById(id);
    if (matcherTableElement && matcherRow) {
        matcherTableElement.removeChild(matcherRow);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const matcherTableElement = document.getElementById('matcherTableBody');
    if (matcherTableElement) {
        for (let i = 0; i < 10; i++) {
            let id = generateRandomId(10);
            let matcher = new Matcher({
                id: id,
                name: id,
                pattern: '/',
                ignoreCase: true,
                onClick: removeMatcher,
            });
            matcherTableElement.appendChild(matcher.render());
        }
    }
});