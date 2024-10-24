import gulp from 'gulp';
import clean from 'gulp-clean';
import path from 'path';
import { fileURLToPath } from 'url';

// Get the directory name of the current module
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Define paths
const paths = {
  src: 'src/**/*', // Source directory
  dest: path.join(__dirname, './../../Knowit.Umbraco.TokenReplacement.Backend/ui'), // Destination directory
};

// Task to clean the destination folder
gulp.task('clean', function () {
  console.log(`Cleaning the destination folder: ${paths.dest}`);
  return gulp.src(paths.dest, { allowEmpty: true, read: false })
    .pipe(clean({ force: true }))
    .on('end', () => console.log('Cleaning completed.'));
});

// Task to copy files from src to dest
gulp.task('copy', function () {
  console.log(`Copying files from ${paths.src} to ${paths.dest}`);
  return gulp.src(paths.src)
    .pipe(gulp.dest(paths.dest))
    .on('end', () => console.log('Files copied successfully.'));
});

// Run clean and copy tasks in sequence
gulp.series('clean', 'copy')(() => {
  console.log('Build process completed.');
});

//
