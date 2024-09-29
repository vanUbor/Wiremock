import {promises as fs} from 'fs';

async function readFile(): Promise<string> {

    return new Promise<string>((resolve, reject) => {
        try {
            console.log('Reading file...');
            let data = fs.readFile('../Scratches/mapping.json', 'utf-8');
            data.then(json => {
                if (json.charCodeAt(0) === 0xFEFF) {
                    resolve(json.slice(1));
                } else {
                    resolve(json);
                }
            });
        }catch (error) {
            reject('Error reading the file: ' + error);
        }
    });
}

let p = readFile();
p.then((result) => {
    let o : any = JSON.parse((result))
    console.log(o);
});

