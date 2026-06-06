import { readFileSync, readdirSync, statSync } from 'node:fs'
import { join } from 'node:path'
import { fileURLToPath } from 'node:url'

const distDir = fileURLToPath(new URL('../dist', import.meta.url))
const forbiddenPatterns = ['localhost:5167', 'localhost:5173']

function collectFiles(dir) {
  const entries = readdirSync(dir)
  const files = []

  for (const entry of entries) {
    const fullPath = join(dir, entry)
    const stats = statSync(fullPath)

    if (stats.isDirectory()) {
      files.push(...collectFiles(fullPath))
      continue
    }

    if (entry.endsWith('.js') || entry.endsWith('.css')) {
      files.push(fullPath)
    }
  }

  return files
}

function verifyBuild() {
  const files = collectFiles(distDir)
  const violations = []

  for (const file of files) {
    const content = readFileSync(file, 'utf8')

    for (const pattern of forbiddenPatterns) {
      if (content.includes(pattern)) {
        violations.push({ file, pattern })
      }
    }
  }

  if (violations.length > 0) {
    console.error('Build verification failed. Dev URLs found in production bundle:')
    for (const { file, pattern } of violations) {
      console.error(`  - "${pattern}" in ${file}`)
    }
    process.exit(1)
  }

  console.log(`Build verification passed (${files.length} files scanned).`)
}

verifyBuild()
