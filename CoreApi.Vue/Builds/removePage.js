import fs from "fs"
import path from "path"

var pageName = "mpapage";
process.argv.forEach(function (val, index, array) {
    if (index === 2) pageName = val;
});

const templatesPath = `../Templates/${pageName}`;
const viewsPath = `../Views/${pageName}`;
const viewTemplatePath = `../clientApps/operation/views`;
const controllerPath = `../controllers`;
const controllerTemplatePath = `../clientApps/operation/controllers`;
const clientMpaPagePath = `../clientApps/src/pages/${pageName}`;
const clientMpaPageTemplatePath = `../clientApps/operation/mpapage`;
const clientSpaPagePath = `../clientApps/src/components/${pageName}/contents`;
const clientSpaPageTemplatePath = `../clientApps/operation/spapage/contents`;

var pairPaths = [
    { source: viewTemplatePath, targets: [viewsPath, templatesPath], isValidTarget: true, isFileNameCase: true, isContentCase: false },
    { source: controllerTemplatePath, targets: [controllerPath], isValidTarget: false, isFileNameCase: true, isContentCase: true },
    { source: clientMpaPageTemplatePath, targets: [clientMpaPagePath], isValidTarget: true, isFileNameCase: false, isContentCase: false },
    { source: clientSpaPageTemplatePath, targets: [clientSpaPagePath], isValidTarget: true, isFileNameCase: false, isContentCase: false }
];

const exProcess = (e) => {
    if (e) console.log(e)
}
function validPair(pairPaths) {
    var result = { mapping: [], message: [], isvalid: true };
    pairPaths.forEach((p) => {
        if (!fs.existsSync(p.source)) {
            result.message.push(`${p.source}未存在`);
            return;
        }
        if (p.isValidTarget)
            p.targets.forEach(t => {
                if (fs.existsSync(t)) {
                    result.message.push(`${t}已存在`);
                    return;
                }
            });
        var sources = fs.readdirSync(p.source);
        sources.forEach(s => {
            p.targets.forEach(t => {
                var sourceFileInfo = path.parse(s);
                //console.log(sourceFileInfo);
                var source = `${p.source}/${s}`;
                var caseName = p.isFileNameCase ? toCamel(pageName) : pageName;
                var target = `${t}/${sourceFileInfo.base}`.replaceAll('mpapage', caseName).replaceAll(pageName, caseName);
                result.mapping.push({ source, target , pair:p });
                if (fs.existsSync(target.replaceAll(".temp",""))) result.message.push(`${target}已存在`);
            });
        });
    })
    if (result.message.length) {
        result.isvalid = false;
        result.mapping.splice(0);
    }
    result.message.forEach(c => console.log(c));
    return result;
}

function createPair(mapping) {
    mapping.forEach(p => {
        var content = fs.readFileSync(p.source).toString();
        var caseName = p.pair.isContentCase ? toCamel(pageName) : pageName;
        content = content.replaceAll("mpapage", caseName);
        var targetInfo = path.parse(p.target);
        fs.mkdirSync(targetInfo.dir, { recursive: true })
        fs.writeFileSync(p.target.replace(".temp",""),content);
    });
}

function toCamel(camelStr = '')
{
    var name = camelStr.replace(/-([a-z])/g, function (all, i) {
        return i.toUpperCase();
    });
    name = `${name[0].toUpperCase()}${name.substring(1)}`;
    return name;
}

function main() {
    console.log(`開始檢查新頁面${pageName}`);
    var validResult = validPair(pairPaths);
    if (!validResult.isvalid) return;
    console.log(`開始建立新頁面${pageName}`);
    createPair(validResult.mapping);
    addConfig();
}

function addConfig() {
    var content = fs.readFileSync(`../clientApps/app.config.json`).toString();
    var config = JSON.parse(content);
    config.apps.push(
        {
            "devUrl": `wwwroot/templates/${pageName}/${pageName}.html`,
            "name": `${pageName}`,
            "entry": `../Templates/${toCamel(pageName)}/Index.cshtml`,
            "output": `../Views/${toCamel(pageName)}/Index.cshtml`
        });
    fs.writeFileSync(`../clientApps/app.config.json`, JSON.stringify(config));
}
main();