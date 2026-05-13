# AGENTS.md

> Карта проекта для ИИ-агентов. Поддерживайте этот файл в актуальном состоянии по мере развития проекта.

## Project Overview
Проект сфокусирован на реализации многоуровневой системы ИИ на базе GOAP для авто-батлера, где юниты действуют автономно.

## Tech Stack
| Технология | Решение |
|------------|---------|
| Engine     | Unity 6000.0.58f2 (URP) |
| DI         | Zenject |
| Async      | UniTask |
| UI         | uGUI + MVVM |
| Tweens     | DOTween |

## Project Structure
```
Assets/
├── Game/Scripts/          [Основная логика игры]
│   ├── Config/            [Конфигурации и ScriptableObjects]
│   ├── GOAP/              [Ядро системы GOAP]
│   ├── Installers/        [Zenject Installers]
│   ├── Units/             [Логика юнитов и AIAgent]
│   └── ...
├── Modules/               [Модульные системы]
│   ├── UI/                [Система пользовательского интерфейса]
│   └── UIBase/            [Базовые компоненты UI]
├── Plugins/               [Сторонние плагины (DOTween и др.)]
├── Resources/             [Глобальные ресурсы]
├── Settings/              [Настройки графики и URP]
└── Scenes/                [Файлы сцен]
Packages/                  [Зависимости UPM]
ProjectSettings/           [Настройки проекта Unity]
.unikit/                   [Контекст ИИ-агента]
```

## Key Entry Points
| File | Purpose |
|------|---------|
| Assets/Game/Scenes/BootstrapScene.unity | Точка входа в приложение |
| Assets/Game/Scenes/GameScene.unity | Основная сцена геймплея |
| Assets/Game/Scripts/Installers/ | Конфигурация DI (Composition Root) |
| Packages/manifest.json | Зависимости пакетов |

## Scenes
| Scene | Purpose |
|-------|---------|
| BootstrapScene | Инициализация систем и переход |
| GameScene | Основной игровой цикл и битва юнитов |
| MainMenuScene | Главное меню игры |

## Module Map
| Module | Path | Purpose |
|--------|------|---------|
| UI | Assets/Modules/UI | Система управления окнами и элементами интерфейса |
| UIBase | Assets/Modules/UIBase | Фундаментальные классы для UI модулей |
| GOAP | Assets/Game/Scripts/GOAP | Реализация планировщика и действий ИИ |

## AI Context Files
| File | Purpose |
|------|---------|
| AGENTS.md | Этот файл — карта структуры проекта |
| .unikit/config.yaml | Настраиваемый конфиг UniKit (пути, язык, git) |
| .unikit/DESCRIPTION.md | Спецификация проекта и техстек |
| .unikit/ARCHITECTURE.md | Архитектурные решения и рекомендации |
| .unikit/system/LANGUAGE_RULES.md | Правила обработки языков |
