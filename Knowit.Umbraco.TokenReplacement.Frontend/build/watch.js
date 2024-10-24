import { readdir } from 'fs/promises';
import path from 'path';
import { fileURLToPath } from 'url';
import inquirer from 'inquirer';
import gulp from 'gulp';
import clean from 'gulp-clean';
import copy from 'gulp-copy';

// Get the directory name of the current module
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Get the folder path and app plugins directory from command-line arguments
const args = process.argv.slice(2); // Skip the first two default arguments
const baseFolderPath = path.join(__dirname, args[0]);
const appPluginsPath = args[1] || 'app_plugins'; // Default to 'app_plugins' if not provided

// Function to list all folders in a directory
async function listFolders(directoryPath) {
  try {
    const files = await readdir(directoryPath, { withFileTypes: true });

    // Filter out directories from the list of files
    const folders = files
      .filter(dirent => dirent.isDirectory())
      .map(dirent => dirent.name);

    return folders;
  } catch (err) {
    console.error('Unable to scan directory:', err);
    return [];
  }
}

// Function to prompt user to select a folder or auto-select if only one
async function selectFolder(folders) {
  if (folders.length === 1) {
    console.log(`Only one folder found: ${folders[0]}. Auto-selecting it.`);
    return folders[0];
  }

  const answer = await inquirer.prompt([
    {
      type: 'list',
      name: 'selectedFolder',
      message: 'Please select a folder:',
      choices: folders,
    },
  ]);

  console.log(`You selected: ${answer.selectedFolder}`);
  return answer.selectedFolder;
}

// Function to run Gulp watch task
function runGulpWatch(selectedFolder) {
  // Construct the outDir path using the selected folder and app plugins directory
  const outDir = path.join(baseFolderPath, selectedFolder, appPluginsPath);

  // Define paths for Gulp
  const paths = {
    src: 'src/**/*', // Source directory to watch
    dest: outDir, // Destination directory to copy to
  };

  // Clean the destination folder (with force to allow deleting files outside the working directory)
  gulp.task('clean', function () {
    console.log(`Cleaning the destination folder: ${paths.dest}`);
    return gulp.src(paths.dest, { allowEmpty: true, read: false })
      .pipe(clean({ force: true }))
      .on('end', () => console.log('Cleaning completed.'));
  });

  // Copy files from src to dest
  gulp.task('copy', function () {
    console.log(`Copying files from ${paths.src} to ${paths.dest}`);
    return gulp.src(paths.src)
      .pipe(gulp.dest(paths.dest))
      .on('end', () => console.log('Files copied successfully.'));
  });

  // Watch task to watch for changes and clean + copy files
  gulp.task('watch', function () {
    console.log(`Watching for changes in: ${paths.src}`);
    gulp.watch(paths.src, gulp.series('clean', 'copy'))
      .on('change', (path) => console.log(`File changed: ${path}`))
      .on('add', (path) => console.log(`File added: ${path}`))
      .on('unlink', (path) => console.log(`File removed: ${path}`));
  });

  // Start the watch task
  gulp.series('clean', 'copy', 'watch')();
}

// Main function to execute the script
async function main() {
  const folders = await listFolders(baseFolderPath);
  if (folders.length > 0) {
    const selectedFolder = await selectFolder(folders);
    runGulpWatch(selectedFolder);
  } else {
    console.log('No folders found.');
  }
}

main();
