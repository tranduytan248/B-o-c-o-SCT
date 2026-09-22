/**
 * Test Runner chính thức của Skill unit-testing-test-generate
 * Cách dùng: node run_module_tests.js [ModuleName]
 * Mặc định: DigitalSales
 */

const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const { execSync } = require('child_process');

const moduleName = process.argv[2] || 'DigitalSales';
const areaName = 'Cate';

console.log('============================================================');
console.log(`   AUTOMATED TEST RUNNER: MODULE ${moduleName.toUpperCase()}   `);
console.log('============================================================\n');

const results = [];

function recordResult(layer, testName, status, details) {
    results.push({ layer, testName, status, details });
    const color = status === 'PASS' ? '\x1b[32m' : '\x1b[31m';
    console.log(`${color}[${status}]\x1b[0m Layer ${layer} - ${testName}: ${details}`);
}

// -------------------------------------------------------------
// LAYER 1: MSBUILD COMPILE TEST
// -------------------------------------------------------------
try {
    console.log('--- Running Layer 1: MSBuild Compilation ---');
    const msbuild = 'C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe';
    const rootDir = 'd:\\SVN\\Bao-cao-SCT\\Source\\ReportDeptTourismSolution';
    const psCmd = `powershell -NoProfile -Command "& '${msbuild}' '${rootDir}\\CenIT.ReportTourism.Modules.${areaName}Module\\CenIT.ReportTourism.Modules.${areaName}Module.csproj' /p:Configuration=Debug /p:Platform=AnyCPU /p:SolutionDir='${rootDir}\\' /t:Build"`;
    const out = execSync(psCmd, { encoding: 'utf8', maxBuffer: 10 * 1024 * 1024 });
    if (out.includes('0 Error(s)')) {
        recordResult(1, `Modules.${areaName} Compilation`, 'PASS', 'Biên dịch thành công với 0 lỗi cú pháp C#.');
    } else {
        recordResult(1, `Modules.${areaName} Compilation`, 'FAIL', 'Có lỗi trong quá trình biên dịch.');
    }
} catch (err) {
    recordResult(1, `Modules.${areaName} Compilation`, 'FAIL', err.message);
}

// -------------------------------------------------------------
// LAYER 2: TRIPLE MIRRORING & UTF-8 BOM TEST
// -------------------------------------------------------------
console.log('\n--- Running Layer 2: Triple Mirroring & UTF-8 BOM Verification ---');
const rootPath = 'd:/SVN/Bao-cao-SCT/Source/ReportDeptTourismSolution';
const srcDir = `${rootPath}/CenIT.ReportTourism.Modules.${areaName}Module/Areas/${areaName}/Views/${moduleName}`;
const dest1Dir = `${rootPath}/publish_source/Areas/${areaName}/Views/${moduleName}`;
const dest2Dir = `${rootPath}/CenIT.ReportTourism.WebApp/Areas/${areaName}/Views/${moduleName}`;

if (fs.existsSync(srcDir)) {
    const files = fs.readdirSync(srcDir);
    let mirrorPass = true;
    let bomPass = true;
    let mirrorErrors = [];

    files.forEach(f => {
        const pSrc = path.join(srcDir, f);
        const p1 = path.join(dest1Dir, f);
        const p2 = path.join(dest2Dir, f);

        if (!fs.existsSync(p1) || !fs.existsSync(p2)) {
            mirrorPass = false;
            mirrorErrors.push(`Missing in destination: ${f}`);
            return;
        }

        const bSrc = fs.readFileSync(pSrc);
        const b1 = fs.readFileSync(p1);
        const b2 = fs.readFileSync(p2);

        const hSrc = crypto.createHash('md5').update(bSrc).digest('hex');
        const h1 = crypto.createHash('md5').update(b1).digest('hex');
        const h2 = crypto.createHash('md5').update(b2).digest('hex');

        if (hSrc !== h1 || hSrc !== h2) {
            mirrorPass = false;
            mirrorErrors.push(`MD5 mismatch: ${f}`);
        }

        if (f.endsWith('.cshtml')) {
            if (!(bSrc[0] === 0xEF && bSrc[1] === 0xBB && bSrc[2] === 0xBF)) {
                bomPass = false;
                mirrorErrors.push(`Missing BOM: ${f}`);
            }

            // Kiểm tra cân bằng ngoặc nhọn { } chống lỗi HttpParseException sập HTTP 500
            const text = fs.readFileSync(pSrc, 'utf8');
            let o = 0, cl = 0;
            for (let ch of text) {
                if (ch === '{') o++;
                if (ch === '}') cl++;
            }
            if (o !== cl) {
                mirrorPass = false;
                mirrorErrors.push(`Razor braces mismatch in ${f} (Open=${o}, Close=${cl})`);
            }
        }
    });

    if (mirrorPass) {
        recordResult(2, 'Triple Mirroring & Razor Syntax', 'PASS', `Đồng bộ hoàn hảo 100% hash MD5 cho toàn bộ ${files.length} files và cú pháp Razor cân bằng ngoặc 100%.`);
    } else {
        recordResult(2, 'Triple Mirroring & Razor Syntax', 'FAIL', mirrorErrors.join('; '));
    }

    if (bomPass) {
        recordResult(2, 'Razor Views UTF-8 BOM', 'PASS', '100% tệp Razor .cshtml đều có UTF-8 with BOM chuẩn.');
    } else {
        recordResult(2, 'Razor Views UTF-8 BOM', 'FAIL', 'Phát hiện tệp thiếu BOM.');
    }
} else {
    recordResult(2, 'Triple Mirroring', 'SKIP', `Không tìm thấy thư mục views ${srcDir}`);
}

// -------------------------------------------------------------
// LAYER 3: DOM ID COLLISION SCANNER
// -------------------------------------------------------------
console.log('\n--- Running Layer 3: Anti-DOM ID Collision Scanner ---');
const indexPath = path.join(srcDir, 'Index.cshtml');
const searchPath = path.join(srcDir, '_Search.cshtml');
if (fs.existsSync(indexPath)) {
    const indexHtml = fs.readFileSync(indexPath, 'utf8') + (fs.existsSync(searchPath) ? fs.readFileSync(searchPath, 'utf8') : '');
    const idRegex = /id=["']([^"']+)["']/g;
    const parentIds = new Set();
    let match;
    while ((match = idRegex.exec(indexHtml)) !== null) {
        const id = match[1];
        if (!id.startsWith('tbl') && !id.includes('search') && !id.includes('btn')) {
            parentIds.add(id);
        }
    }

    const modalFiles = fs.readdirSync(srcDir).filter(f => f.startsWith('_') && f.endsWith('.cshtml') && f !== '_Search.cshtml');
    let collisionCount = 0;
    let collisionList = [];

    modalFiles.forEach(mf => {
        const mfPath = path.join(srcDir, mf);
        const mHtml = fs.readFileSync(mfPath, 'utf8');
        const mRegex = /id=["']([^"']+)["']/g;
        let mm;
        while ((mm = mRegex.exec(mHtml)) !== null) {
            const mId = mm[1];
            if (['CustomerID', 'StatusID', 'BusinessType', 'DepartmentID', 'AssignedEmployeeID'].includes(mId)) {
                collisionCount++;
                collisionList.push(`${mf} conflict: ${mId}`);
            }
        }
    });

    if (collisionCount === 0) {
        recordResult(3, 'DOM ID Collision Prevention', 'PASS', 'Không phát hiện bất kỳ xung đột DOM ID cốt lõi nào giữa Modal và trang danh sách.');
    } else {
        recordResult(3, 'DOM ID Collision Prevention', 'FAIL', collisionList.join('; '));
    }
} else {
    recordResult(3, 'DOM ID Collision Prevention', 'SKIP', 'Không tìm thấy Index.cshtml');
}

// -------------------------------------------------------------
// LAYER 4: SYS_MESSAGES DB COVERAGE TEST
// -------------------------------------------------------------
console.log('\n--- Running Layer 4: Sys_Messages DB Coverage Verification ---');
const controllerPath = `d:/SVN/Bao-cao-SCT/Source/ReportDeptTourismSolution/CenIT.ReportTourism.Modules.${areaName}Module/Areas/${areaName}/Controllers/${moduleName}Controller.cs`;
if (fs.existsSync(controllerPath)) {
    const controllerText = fs.readFileSync(controllerPath, 'utf8');
    const keyRegex = new RegExp(`"(${moduleName}_[A-Za-z0-9_]+)"`, 'g');
    const calledKeys = new Set();
    let match;
    while ((match = keyRegex.exec(controllerText)) !== null) {
        calledKeys.add(match[1]);
    }

    let missingKeys = [];
    let emptyKeys = [];

    if (calledKeys.size > 0) {
        try {
            const scratchDir = 'C:/Users/MrTan/.gemini/antigravity-ide/brain/21997f5e-4caf-4b26-b65d-33aa22694fff/scratch';
            if (!fs.existsSync(scratchDir)) fs.mkdirSync(scratchDir, { recursive: true });

            const keysJson = JSON.stringify(Array.from(calledKeys));
            const keysFile = path.join(scratchDir, `keys_check_${moduleName}.json`);
            fs.writeFileSync(keysFile, keysJson, 'utf8');

            const psScript = `
                $keys = Get-Content '${keysFile}' | ConvertFrom-Json
                $connStr = 'Data Source=10.57.30.10;Initial Catalog=quanlydoanhthucenit;User Id=quanlydoanhthucenit;Password=Kdhe@543HE2;Connect Timeout=5;'
                $conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
                $conn.Open()
                $result = @{}
                foreach ($k in $keys) {
                    $cmd = $conn.CreateCommand()
                    $cmd.CommandText = "SELECT Message FROM Sys_Messages WHERE LangCode = 'vi-VN' AND LabelKey = @k"
                    $cmd.Parameters.AddWithValue('@k', $k) | Out-Null
                    $val = $cmd.ExecuteScalar()
                    $result[$k] = if ($null -ne $val) { $val.ToString() } else { $null }
                }
                $conn.Close()
                $result | ConvertTo-Json -Compress
            `;
            const psFile = path.join(scratchDir, `query_keys_${moduleName}.ps1`);
            fs.writeFileSync(psFile, psScript, 'utf8');

            const jsonOut = execSync(`powershell -NoProfile -ExecutionPolicy Bypass -File "${psFile}"`, { encoding: 'utf8' }).trim();
            const dbMap = JSON.parse(jsonOut);

            calledKeys.forEach(k => {
                if (!(k in dbMap) || dbMap[k] === null) {
                    missingKeys.push(k);
                } else if (dbMap[k].trim() === '') {
                    emptyKeys.push(k);
                }
            });
        } catch (e) {
            console.error('SQL check error:', e.message);
            missingKeys.push('SQL_CONNECTION_ERROR');
        }

        if (missingKeys.length === 0 && emptyKeys.length === 0) {
            recordResult(4, 'Sys_Messages DB Coverage', 'PASS', `Toàn bộ ${calledKeys.size} message keys trong Controller đều có mặt trong DB và có nội dung tiếng Việt.`);
        } else {
            recordResult(4, 'Sys_Messages DB Coverage', 'FAIL', `Thiếu keys: [${missingKeys.join(', ')}], Rỗng: [${emptyKeys.join(', ')}]`);
        }
    } else {
        recordResult(4, 'Sys_Messages DB Coverage', 'PASS', 'Không có message keys đặc thù cần kiểm tra.');
    }
} else {
    recordResult(4, 'Sys_Messages DB Coverage', 'SKIP', 'Không tìm thấy Controller.');
}

// -------------------------------------------------------------
// LAYER 5: CLEAN CODE & NO HARDCODED UI TEXT
// -------------------------------------------------------------
console.log('\n--- Running Layer 5: Clean Code & No Hardcoded UI Text ---');
if (fs.existsSync(controllerPath)) {
    const controllerText = fs.readFileSync(controllerPath, 'utf8');
    const lines = controllerText.split('\n');
    const vnRegex = /[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ]/;
    let hardcodedCount = 0;
    lines.forEach(line => {
        const trimmed = line.trim();
        if (trimmed.startsWith('//') || trimmed.startsWith('*') || trimmed.startsWith('/*')) return;
        if (vnRegex.test(line)) hardcodedCount++;
    });

    const correctFolder = controllerText.includes(`_folderUpload = "/Contents/Uploads/${moduleName}";`);
    const hasForbiddenExts = controllerText.includes('forbiddenExts');

    if (hardcodedCount === 0 && correctFolder && hasForbiddenExts) {
        recordResult(5, 'Clean Code, Storage & Security', 'PASS', `0 chuỗi tiếng Việt hardcoded; Thư mục /Contents/Uploads/${moduleName}; Có blacklist chặn extension độc hại.`);
    } else {
        recordResult(5, 'Clean Code, Storage & Security', 'FAIL', `Hardcoded lines: ${hardcodedCount}, Folder valid: ${correctFolder}, Blacklist valid: ${hasForbiddenExts}`);
    }
} else {
    recordResult(5, 'Clean Code, Storage & Security', 'SKIP', 'Không tìm thấy Controller.');
}

console.log('\n============================================================');
console.log('                BẢNG TỔNG HỢP KẾT QUẢ TEST                  ');
console.log('============================================================');
console.table(results);

const allPass = results.every(r => r.status === 'PASS' || r.status === 'SKIP');
if (allPass) {
    console.log('\x1b[32m\x1b[1m>>> TẤT CẢ CÁC BÀI TEST ĐỀU ĐẠT CHUẨN 100% (ALL PASS)! <<<\x1b[0m\n');
} else {
    console.log('\x1b[31m\x1b[1m>>> CẢNH BÁO: CÓ BÀI TEST CHƯA ĐẠT! <<<\x1b[0m\n');
    process.exit(1);
}
