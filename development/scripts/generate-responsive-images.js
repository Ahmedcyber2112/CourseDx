// development/scripts/generate-responsive-images.js
const fs = require('fs-extra');
const path = require('path');
const Sharp = require('sharp');

// Input and output directories (relative to project root)
const inputDir = path.resolve('./wwwroot/img');
const outputDir = path.resolve('./wwwroot/img');

// Target widths for responsive images
const sizes = [400, 800, 1200];

// Formats to generate
const formats = ['webp', 'avif'];

// Supported input formats
const supportedFormats = /\.(jpg|jpeg|png|webp)$/i;

async function processImage(filePath, filename) {
    try {
        const fileBuffer = await fs.readFile(filePath);
        const basename = path.basename(filename, path.extname(filename));

        console.log(`Processing: ${filename}`);

        // Process each size and format
        for (const size of sizes) {
            const dir = path.join(outputDir, `${size}w`);
            await fs.ensureDir(dir);

            for (const format of formats) {
                const outPath = path.join(dir, `${basename}-${size}.${format}`);
                await Sharp(fileBuffer)
                    .resize(size)
                    .toFormat(format, { quality: 80 })
                    .toFile(outPath);

                console.log(`✓ Generated: ${outPath}`);
            }
        }
    } catch (err) {
        console.error(`❌ Error processing ${filename}:`, err.message);
    }
}

async function main() {
    try {
        // Ensure input directory exists
        if (!await fs.pathExists(inputDir)) {
            console.log(`⚠️  Input directory not found: ${inputDir}`);
            return;
        }

        const files = await fs.readdir(inputDir);

        // Filter root-level image files only (exclude subdirectories)
        const imageFiles = files.filter(file => {
            const fullPath = path.join(inputDir, file);
            const stat = fs.statSync(fullPath);
            return stat.isFile() && supportedFormats.test(file);
        });

        if (imageFiles.length === 0) {
            console.log('ℹ️  No images found to process');
            return;
        }

        console.log(`Found ${imageFiles.length} images to process...\n`);

        // Filter out files that are already processed (ending with -400.webp, -800.avif, etc.)
        const rootImages = imageFiles.filter(f => {
            // Check if filename ends with -SIZE.format pattern
            const isProcessed = sizes.some(size =>
                new RegExp(`-${size}\\.(webp|avif)$`).test(f)
            );
            return !isProcessed;
        });

        if (rootImages.length === 0) {
            console.log('ℹ️  No root images found to process (skipping already processed files)');
            return;
        }

        console.log(`Processing ${rootImages.length} root images...\n`);

        for (const file of rootImages) {
            const filePath = path.join(inputDir, file);
            await processImage(filePath, file);
        }

        console.log('\n✅ All responsive images generated successfully!');
        console.log('\n📁 Output structure:');
        console.log('wwwroot/img/');
        console.log('├── c-1.webp');
        console.log('├── c-2.webp');
        console.log('├── 400w/');
        console.log('│   ├── c-1-400.webp');
        console.log('│   ├── c-1-400.avif');
        console.log('│   ├── c-2-400.webp');
        console.log('│   └── c-2-400.avif');
        console.log('├── 800w/');
        console.log('│   ├── c-1-800.webp');
        console.log('│   ├── c-1-800.avif');
        console.log('│   ├── c-2-800.webp');
        console.log('│   └── c-2-800.avif');
        console.log('└── 1200w/');
        console.log('    ├── c-1-1200.webp');
        console.log('    ├── c-1-1200.avif');
        console.log('    ├── c-2-1200.webp');
        console.log('    └── c-2-1200.avif');

    } catch (err) {
        console.error('❌ Error:', err.message);
        process.exit(1);
    }
}

main();