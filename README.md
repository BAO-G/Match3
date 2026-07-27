# Match-3

一个基于 Unity 的 3-Match 三消益智游戏。

## 项目特点

- 网格棋盘三消系统
- 支持多种棋子类型（普通、泡泡等）
- 棋子颜色与移动逻辑
- 基于 Unity URP 2D 渲染

## 环境要求

- Unity 2022.3+ (URP 2D 模板)
- uv 包管理器 (用于 Unity-MCP)

## 快速开始

1. 克隆本项目
2. 在 Unity Hub 中打开项目目录
3. 打开 `Assets/Scenes/SampleScene.unity` 启动游戏

## 项目结构

```
Assets/
├── Scenes/          # 游戏场景
├── Scripts/         # 核心脚本
│   ├── GridSystem.cs     # 棋盘网格系统
│   ├── GamePiece.cs      # 棋子基类
│   ├── ColorPiece.cs     # 棋子颜色组件
│   └── MovablePiece.cs   # 棋子移动组件
└── Settings/        # 项目设置
```