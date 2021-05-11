#pragma once
#include "Window.h"
#include "Timer.h"
#include "ImguiManager.h"
#include "Camera.h"
#include "PointLight.h"
#include <set>

class App
{
public:
	App();
	// Master frame / message loop
	int Go();
	~App();
private:
	void DoFrame();
	void SpawnSimulationWindow() noexcept;
	void SpawnBoxWindowManager() noexcept;
	void SpawnBoxWindows() noexcept;
private:
	ImguiManager imgui;
	Window wnd;
	Timer timer;
	std::vector<std::unique_ptr<class Drawable>> drawables;
	std::vector<class Box*> boxes;
	float simulation_speed = 1.0f;
	Camera cam;
	PointLight light;
	static constexpr size_t nDrawables = 3;
	std::optional<int> comboBoxIndex;
	std::set<int> boxControlIds;
};