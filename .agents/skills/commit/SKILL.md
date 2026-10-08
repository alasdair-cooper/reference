---
name: commit
description: Create a Git commit when the user asks to commit changes in this repository.
---

# Commit

Create a focused, reviewable commit containing only the intended changes.

## Workflow

1. Read applicable repository instructions.
2. Inspect `git --no-optional-locks status --short`, the unstaged diff, and the
   staged diff. Identify changes relevant to the user's request.
3. Do not discard, overwrite, or include unrelated changes. If the intended
   scope is unclear, ask the user before staging or committing.
4. Run checks appropriate to the changes. Report failed or unavailable checks
   honestly; do not claim that checks ran when they did not.
5. Stage intended paths explicitly rather than using `git add .` or `git add -A`.
   If a file mixes intended and unrelated edits, stage only the intended hunks.
6. Review the staged diff before committing, including checking for secrets and
   generated artifacts that should not be committed.
7. Create the commit with `GIT_EDITOR=true git commit` and an explicit message.
   Do not amend an existing commit unless the user explicitly requests it.
8. Inspect the resulting commit and working-tree status. Report the commit hash,
   title, checks performed, and any changes left uncommitted.

## Commit messages

- **Commit titles must be lowercase.** Use lowercase throughout the entire
  subject line, including any type or scope prefix.
- Use a concise, imperative title that describes the change, not the act of
  committing it.
- Do not end the title with a period.
- Prefer titles of 72 characters or fewer.
- Follow any repository convention for prefixes, while keeping them lowercase.
- Add a body when needed to explain the motivation or consequential decisions.
  The lowercase requirement applies to the title, not the body.

Examples:

```text
add commit skill
fix missing validation for empty requests
refactor(api): simplify response mapping
```

## Boundaries

- Do not push, create a pull request, or rewrite shared history unless requested.
- Do not bypass hooks or disable checks to make a commit succeed.
- Do not modify Git identity or configuration without explicit permission.
- If hooks change files, review those changes before staging and retrying.
