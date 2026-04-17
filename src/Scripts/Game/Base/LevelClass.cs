using Godot;
using System;
using System.Collections.Generic;

public partial class LevelClass : Node3D
{
	[Export] private Path3D LevelPathFollow;
	[Export] private AudioStream musicaNivel;
	[Export] private Camera3D ActualCamera;
	[Export] public GpuParticles3D Sparks;

	[Export] private float SpawnInterval = 1.0f;
	[Export] private TextureButton BotonRonda;

	[Export] private PackedScene NormalEnemyScene;
	[Export] private PackedScene FastEnemyScene;
	[Export] private PackedScene TankEnemyScene;



	private float SpawnTimer = 0.0f;
	private PackedScene EnemyScene;
	private MusicManager musicManager;

	private List<DatosNivel> todasLasRondas;
	private DatosNivel datosRondaActual = null;
	public int rondaActualIndice = 0;

	private bool estaCorriendoRonda = false;
	private int indiceOleadaActual = 0;
	private int indiceEnemigoActual = 0;
	private bool esperandoSiguienteOleada = true;

	private float timerSpawns = 0.0f;
	private float timerEntreOleadas = 0.0f;

	private string rutaJson = "res://Scripts/json/nivel1.json";

	public override void _Ready()
	{
		EnemyScene = GD.Load<PackedScene>("res://Scenes/Game/Base/enemy_scene.tscn");
		musicManager = MusicManager.Instance;
		
		if (musicaNivel != null) musicManager.PlayMusic(musicaNivel);

		todasLasRondas = CargadorNiveles.Instance.CargarTodasLasRondas(rutaJson);
		PrepararDatosDeRonda(0);

		//Cargo las escenas de enemigos
		NormalEnemyScene = GD.Load<PackedScene>("res://Scenes/Game/Base/normal_enemy_scene.tscn");
		FastEnemyScene = GD.Load<PackedScene>("res://Scenes/Game/Base/fast_enemy_scene.tscn");
		TankEnemyScene = GD.Load<PackedScene>("res://Scenes/Game/Base/tank_enemy_scene.tscn");

		//Mando la referencia a la camara al GameManager
		GameManager.Instance.SetCamRef(ActualCamera);
		GameManager.Instance.SetLevelRef(this);
	}

	public void OnBotonPasarRondaPressed()
	{
		if (estaCorriendoRonda || datosRondaActual == null) return;

		//GD.Print($"Iniciando Ronda {datosRondaActual.Ronda}");
		estaCorriendoRonda = true;

		if (BotonRonda != null) BotonRonda.Visible = false;
	}

	private void PrepararDatosDeRonda(int indice)
	{
		if (todasLasRondas == null || indice >= todasLasRondas.Count)
		{
			datosRondaActual = null;
			if (BotonRonda != null) BotonRonda.Visible = false;
			return;
		}

		datosRondaActual = todasLasRondas[indice];
		indiceOleadaActual = 0;
		indiceEnemigoActual = 0;
		esperandoSiguienteOleada = true;
		timerEntreOleadas = 0.0f;
		timerSpawns = 0.0f;

		
		if (BotonRonda != null)
		{
			BotonRonda.Visible = true;
			(BotonRonda as BotonPasarRonda).UpdateText(datosRondaActual.Ronda);
		}
	}

	public override void _Process(double delta)
	{
		if (!estaCorriendoRonda || datosRondaActual == null) return;

		bool spawnerFinalizado = indiceOleadaActual >= datosRondaActual.Paquetes.Count;

		if (spawnerFinalizado)
		{

			int enemigosVivos = GetTree().GetNodesInGroup("Enemigos").Count;

			if (enemigosVivos == 0)
			{
				FinalizarRondaActual();
			}
			return;
		}

		if (esperandoSiguienteOleada) ProcesarEsperaOleada(delta);
		else ProcesarSpawnEnemigos(delta);


	}
	private void SpawnEnemy(string tipoEnemigo, int Dificultad)
	{
		var pathFollow = new PathFollow3D();
		pathFollow.Loop = false;
		var enemy = new EnemyClass();

		// 2. Crear Pasajero
		switch (tipoEnemigo)
		{
			case "rapido":
				enemy = FastEnemyScene.Instantiate<EnemyClass>();
				break;

			case "normal":
				enemy = NormalEnemyScene.Instantiate<EnemyClass>();
				break;

			case "lento":
				enemy = TankEnemyScene.Instantiate<EnemyClass>();
				break;
		}

		enemy.SetDifficulty(Dificultad); //Escalamos los enemigos segun la dificultad de la ronda
		enemy.AddToGroup("Enemigos");

		pathFollow.AddChild(enemy);
		LevelPathFollow.AddChild(pathFollow);


		pathFollow.Progress = 0;

		enemy.OnEnemyDied += GameManager.Instance.OnEnemyDied;
		enemy.OnEnemyFinishPath += GameManager.Instance.OnEnemyFinishPath;
	}

	private void ProcesarEsperaOleada(double delta)
	{
		timerEntreOleadas += (float)delta;
		float tiempoNecesario = 0f;
		if (indiceOleadaActual < datosRondaActual.TimingSpawns.Length)
			tiempoNecesario = datosRondaActual.TimingSpawns[indiceOleadaActual];

		if (timerEntreOleadas >= tiempoNecesario)
		{
			esperandoSiguienteOleada = false;
			timerEntreOleadas = 0f;
		}
	}

	private void ProcesarSpawnEnemigos(double delta)
	{
		timerSpawns += (float)delta;
		if (timerSpawns >= SpawnInterval)
		{
			timerSpawns = 0f;
			var paquete = datosRondaActual.Paquetes[indiceOleadaActual];

			if (indiceEnemigoActual < paquete.Count)
			{
				SpawnEnemy(paquete[indiceEnemigoActual], datosRondaActual.Dificultad);
				indiceEnemigoActual++;
			}
			else
			{
				indiceOleadaActual++;
				indiceEnemigoActual = 0;
				esperandoSiguienteOleada = true;
			}
		}
	}

	private void FinalizarRondaActual()
	{
		estaCorriendoRonda = false;
		rondaActualIndice++;
		//GD.Print(rondaActualIndice);


		if (todasLasRondas != null && rondaActualIndice >= todasLasRondas.Count)
		{
			//GD.Print("¡Última ronda completada y mapa limpio!");
			GameManager.Instance.OnWin();
		}
		else
		{
			PrepararDatosDeRonda(rondaActualIndice);
		}
	}

	public int GetRondaActualIndice()
	{
		return rondaActualIndice;
	}

}
