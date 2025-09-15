import fs from "fs"
import path from "path"
import { glob } from "glob"


const exProcess = (e) => {
    if (e) console.log(e)
}

function getPairPaths(pathInfo) {
    var pairPaths = [
        { source: pathInfo.viewTemplatePath, targets: [pathInfo.viewsPath, pathInfo.templatesPath], isValidTarget: true, isFileNameCase: true, isContentCase: false },
        { source: pathInfo.controllerTemplatePath, targets: [pathInfo.controllerPath], isValidTarget: false, isFileNameCase: true, isContentCase: true },
        { source: pathInfo.clientMpaPageTemplatePath, targets: [pathInfo.clientMpaPagePath], isValidTarget: true, isFileNameCase: false, isContentCase: false },
        { source: pathInfo.clientSpaPageTemplatePath, targets: [pathInfo.clientSpaPagePath], isValidTarget: true, isFileNameCase: false, isContentCase: false }
    ];
    return pairPaths;
}

function validPair(pairPaths, pathInfo) {
    let pageName = pathInfo.pageName;
    var result = { mapping: [], message: [], isvalid: true };
    pairPaths.forEach((p) => {
        if (!fs.existsSync(p.source)) {
            result.message.push(`${p.source} 尚未存在`);
            return;
        }
        if (p.isValidTarget)
            p.targets.forEach(t => {
                if (fs.existsSync(t)) {
                    result.message.push(`${t.replaceAll(".temp","")} 已存在`);
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
                if (fs.existsSync(target.replaceAll(".temp", ""))) result.message.push(`${target.replaceAll(".temp", "") } 已存在`);
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

function createPair(mapping, pathInfo) {
    var pageName = pathInfo.pageName;
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

function addConfig(pathInfo) {    
    glob("../clientApps/app.config*.json").then((files) => {
        console.log(files);
        files.forEach(f => {
            console.log(f);
            let pageName = pathInfo.pageName;
            var content = fs.readFileSync(f).toString();
            var config = JSON.parse(content);
            config.apps.push(
                {
                    "devUrl": `wwwroot/templates/${pageName}/${pageName}.html`,
                    "name": `${pageName}`,
                    "entry": `../Templates/${toCamel(pageName)}/Index.cshtml`,
                    "output": `../Views/${toCamel(pageName)}/Index.cshtml`,
                    "publishPath": null,
                    "scssAdditional": null
                });
            fs.writeFileSync(f, JSON.stringify(config));
        });
    });

}

const addApp = function (pathInfo) {
    console.log(`開始建立新頁面${pathInfo.pageName}`);
    var pairPaths = getPairPaths(pathInfo);
    var validResult = validPair(pairPaths, pathInfo);
    if (!validResult.isvalid) return;
    createPair(validResult.mapping, pathInfo);
    addConfig(pathInfo);
    console.log(`新頁面${pathInfo.pageName}建立完成`);
}

const removeApp = function (pathInfo) {
    console.log(`開始清除頁面${pathInfo.pageName}`);
    removePath(pathInfo.templatesPath.replace(pathInfo.pageName, toCamel(pathInfo.pageName)));
    removePath(pathInfo.viewsPath.replace(pathInfo.pageName, toCamel(pathInfo.pageName)));
    removePath(pathInfo.controllerPagePath.replace(pathInfo.pageName, toCamel(pathInfo.pageName)));
    removePath(pathInfo.clientMpaPagePath);
    removePath(pathInfo.clientSpaPagePath);
    removeConfig(pathInfo);
    removePath("../clientApps/templates");
    fs.mkdirSync("../clientApps/templates");
    console.log(`頁面${pathInfo.pageName}清除完成`);
}

function buildPathInfo(pageName, action) {
    var pathInfo = {
        action: action,
        pageName: pageName,
        templatesPath: `../Templates/${pageName}`,
        viewsPath: `../Views/${pageName}`,
        viewTemplatePath: `../clientApps/operation/views`,
        controllerPath: `../controllers`,
        controllerPagePath: `../controllers/${pageName}Controller.cs`,
        controllerTemplatePath: `../clientApps/operation/controllers`,
        clientMpaPagePath: `../clientApps/src/pages/${pageName}`,
        clientMpaPageTemplatePath: `../clientApps/operation/mpapage`,
        clientSpaPagePath: `../clientApps/src/components/${pageName}/contents`,
        clientSpaPageTemplatePath: `../clientApps/operation/spapage/contents`
    }
    return pathInfo;
}

const appAction = function (args)
{
    var pageName = "mpapage";
    var action = "";
    args.forEach(function (val, index, array) {
        if (index === 2) action = val;
        if (index === 3) pageName = val;
    });
    var pathInfo = buildPathInfo(pageName, action);
    if (action == "add") addApp(pathInfo);
    if (action == "remove") removeApp(pathInfo);
}

function removePath(path) {
    fs.rmSync(path, { recursive: true, force: true });
}

function removeConfig(pathInfo) {
    glob("../clientApps/app.config*.json").then((files) => {
        files.forEach(f => {
            let pageName = pathInfo.pageName;
            var content = fs.readFileSync(f).toString();
            var config = JSON.parse(content);
            config.apps = config.apps.filter(f => f.name.toLowerCase() != pathInfo.pageName.toLowerCase());
            fs.writeFileSync(f, JSON.stringify(config));
        });
    });


}

const pageAction = function (args)
{
    var pageName = "spapage";
    var action = "";
    args.forEach(function (val, index, array) {
        if (index === 2) action = val;
        if (index === 3) pageName = val;
    });
    var pathInfo = buildPathInfo(pageName, action);
}

const debugApp = function(args) {

}

export { appAction  }