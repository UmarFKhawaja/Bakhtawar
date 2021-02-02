#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

const parameters = process.argv.slice(2);

const [name, source, target] = parameters;

const properties = JSON.parse(fs.readFileSync(path.join(process.cwd(), `Projects/${name}/Properties/environmentSettings.json`)).toString().trim());

const generateLines = () => {
  return Object.entries(properties).reduce((lines, [key, value]) => {
    if (key === 'ASPNETCORE_ENVIRONMENT') {
      return lines;
    }

    return lines.concat(`${key}="${value}"`);
  }, []);
};

const data = [
  ...generateLines()
].join('\n');

fs.writeFileSync(path.join(process.cwd(), target, `${name}.sysconfig`), data, {
  mode: 0o755
});
