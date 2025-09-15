import fs from "fs"
import path from "path"
import { appAction } from "./core.js"

var action = "";
process.argv.forEach(function (val, index, array) {
    if (index === 2) action = val;
});

function main() {
    if (action == "add" || action == "remove") appAction(process.argv);
}

main();