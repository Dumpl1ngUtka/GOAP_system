# Project: Goap_aa

## Overview
Многопользовательская (по геймплею) игра, где игрок размещает юнитов на карте (аналогично Clash Royale), которые действуют автономно под управлением ИИ. Основная цель — уничтожить главное строение противника, защищая свое. Главный фокус проекта — реализация многоуровневой системы ИИ на базе GOAP.

## Core Features
- **Многоуровневый GOAP:**
    - **AIAgent (Нижний слой):** Юнит, обладающий информацией о себе и ближайшем окружении.
    - **Squad (Промежуточный слой):** Прослойка для передачи приказов.
    - **CommanderAgent (Верхний слой):** Глобальное планирование на основе всей обстановки на карте.
- **Автономное поведение:** Логичное реагирование юнитов на врагов, союзников и интерактивные объекты.
- **Стратегический геймплей:** Размещение юнитов как основная механика взаимодействия игрока.

## Tech Stack
- **Engine:** Unity 6000.0.58f2 / C# / URP
- **DI:** Zenject (Extenject)
- **Async:** UniTask
- **Reactive:** None
- **UI Binding:** MVVM (Custom/Planned)
- **Inspector:** default
- **Animations (UI):** DOTween
- **Asset Loading:** built-in
- **Input:** Unity Input System
- **Navigation:** AI Navigation

## Architecture Notes
- **GOAP-centric:** Система ИИ является ядром архитектуры, разделенным на независимые слои.
- **DI Patterns:** Использование Zenject для управления зависимостями (Installers в `Assets/Game/Scripts/Installers/`).
- **Module Organization:** Разделение на `Game` (основная логика) и `Modules` (переиспользуемые системы типа UI).
- **Asynchronous Logic:** Активное использование UniTask для неблокирующих операций.

## Architecture
See `.unikit/ARCHITECTURE.md` for detailed architecture guidelines.

## Non-Functional Requirements
- **Platform:** Android
- **Performance:** Оптимизация GOAP для работы множества агентов одновременно.
- **Testing:** Использование Unity Test Framework.
- **Documentation:** Поддержка актуального состояния AGENTS.md.
