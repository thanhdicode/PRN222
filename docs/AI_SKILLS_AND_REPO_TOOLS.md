# AI Skills and Repository Tools
# Recommended Tools for PRN222 MangaWorkflow Development

**IMPORTANT**: These are optional productivity recommendations.
Do NOT blindly execute any script from any external repository.
Always inspect README and SKILL.md before installing.
Never allow any skill to modify database schema without explicit team approval.
Never allow any skill to change PRN222 project scope.

---

## Security Rule (Read First)

Before installing any AI skill or plugin:
1. Read the README.md of the repository
2. Read the SKILL.md or AGENTS.md if present
3. Do not run any install scripts that access the internet without inspection
4. Do not allow skills to modify project architecture
5. Do not allow skills to add banned technologies (see AI_AGENT_EXECUTION_RULES.md)

---

## Primary Recommended Tools

### 1. Superpowers (Claude Code Plugin)
Repository: https://github.com/obra/superpowers
Marketplace: https://github.com/obra/superpowers-marketplace

Purpose:
- brainstorming (use before each phase to confirm scope)
- write-plan (help write detailed implementation plans)
- execute-plan (structured plan execution with checkpoints)
- TDD mindset (test-first thinking)
- code-review discipline (review generated code before accepting)

When to use for this project:
- Before implementing each phase: run brainstorming to confirm scope is right
- When reviewing AI agent output before merging
- When planning a complex feature like SignalR or Worker

Install (Claude Code only):
/plugin marketplace add obra/superpowers-marketplace
/plugin install superpowers@superpowers-marketplace

Security: Review the marketplace README before installing. Superpowers is a well-known Claude Code plugin.

---

### 2. Frontend Design Skill
Source: https://github.com/anthropics/claude-code/tree/main/plugins/frontend-design

Purpose:
- Improve UI/UX quality for MVC views, Razor Pages, and Blazor dashboards
- Better Bootstrap layout decisions
- Form usability improvements

When to use:
- After building a new screen, use this skill to suggest layout improvements
- Before finalizing the Blazor dashboard layout

---

### 3. UI UX Pro Max Skill
Repository: https://github.com/nextlevelbuilder/ui-ux-pro-max-skill

Purpose:
- UI/UX design intelligence for role-based screens
- Dashboard design guidance

When to use:
- For final polish of Admin, Mangaka, Assistant, Editor, Board screens
- For dashboard design in Phase 4

Security warning: Inspect the README carefully. Run no install scripts without reading them first.

---

### 4. Interface Design Skill
Repository: https://github.com/Dammyjay93/interface-design

Purpose:
- Consistent UI design engineering across all screens
- Design system consistency

When to use:
- When multiple screens look inconsistent after Phase 2 or 3
- Before final demo preparation

---

### 5. Awesome Claude Skills (Curated List)
Repository: https://github.com/travisvn/awesome-claude-skills

Purpose: Find specific skills for specific tasks.

When to use:
- When looking for a skill to help with a particular problem
- Before writing a custom skill from scratch

---

### 6. Awesome Agent Skills (Cross-agent Catalog)
Repository: https://github.com/VoltAgent/awesome-agent-skills

Purpose: Cross-agent skill catalog compatible with Claude Code, Codex, Cursor, Gemini CLI, Aider.

When to use:
- When working with multiple AI tools and need skills that work across them

---

### 7. GitHub Awesome Copilot
Repository: https://github.com/github/awesome-copilot

Purpose: Custom instructions, agents, hooks, workflows, plugins for GitHub Copilot.

When to use:
- To improve .github/copilot-instructions.md
- To add project-specific Copilot instructions for MVC patterns

---

### 8. Context Engineering Intro
Repository: https://github.com/coleam00/context-engineering-intro

Purpose: Context engineering template. Helps structure AGENTS.md, docs, and phase prompts better.

When to use:
- After Phase 2 if AGENTS.md needs improvement
- When the AI agent keeps forgetting scope rules

---

### 9. Product Manager Skills
Repository: https://github.com/deanpeters/Product-Manager-Skills

Purpose: Product planning and feature definition.

When to use:
- When refining user stories for Phase 3 or 4
- When writing acceptance criteria for grading review

---

### 10. Claude Code for PMs
Repository: https://github.com/lautarogiambroni/claude-code-for-pms

Purpose: PM skills for planning features and managing execution.

When to use:
- When converting phase plans into executable feature slices
- When managing the execution timeline for the team

---

## How to Decide Which Tool to Use

| Situation | Recommended Tool |
|---|---|
| About to start a new phase | Superpowers brainstorming |
| Reviewing AI-generated code | Superpowers code-review |
| UI looks bad after implementing a screen | Frontend Design Skill or Interface Design |
| Dashboard needs polish | UI UX Pro Max |
| Need a specific automation skill | Awesome Claude Skills or Awesome Agent Skills |
| Agent keeps going off-scope | Context Engineering Intro |
| Writing user stories for the report | Product Manager Skills |

---

## What NOT to Install

Do not install any skill that:
- Modifies database schema automatically
- Adds banned technologies (CQRS, Docker, microservices, etc.)
- Runs unknown shell scripts without your review
- Requires cloud API keys you do not control
- Changes the project architecture beyond Domain/Application/Infrastructure/Web/Worker
