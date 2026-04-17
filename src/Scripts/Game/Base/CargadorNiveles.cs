using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

public class DatosNivel
{
    public int Ronda { get; set; }
    public int Dificultad { get; set; }
    public List<List<string>> Paquetes { get; set; }
    public float[] TimingSpawns { get; set; }
}

public partial class CargadorNiveles : Node
{
    public static CargadorNiveles Instance;

    //Lista con todas las rondas
    public List<DatosNivel> TodasLasRondas { get; private set; } = new List<DatosNivel>();

    public override void _Ready()
    {
        Instance = this;
        //Se cargan aqui todas las rondas del json dado
        TodasLasRondas = CargarTodasLasRondas("res://Scripts/json/nivel1.json");
    }

    public List<DatosNivel> CargarTodasLasRondas(string ruta)
    {
        if (!FileAccess.FileExists(ruta))
        {
            //GD.PrintErr($"No se encontró el archivo en: {ruta}");
            return new List<DatosNivel>();
        }

        using var archivo = FileAccess.Open(ruta, FileAccess.ModeFlags.Read);
        string contenidoJson = archivo.GetAsText();

        try
        {
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };


            var lista = JsonSerializer.Deserialize<List<DatosNivel>>(contenidoJson, opciones);

            //GD.Print($"--- {lista.Count} Rondas cargadas exitosamente ---");
            // se returnea una lista con todas las rondas y sus datos
            return lista;
        }
        catch (Exception e)
        {
            //GD.PrintErr($"Error al leer el JSON: {e.Message}");
            return new List<DatosNivel>();
        }
    }

    public DatosNivel ObtenerRonda(int numeroRonda)
    {
        //Busca y devuelve la ronda con el numero especificado y si no la encuentra devuelve el numero de ronda.
        return TodasLasRondas.FirstOrDefault(r => r.Ronda == numeroRonda);
    }
}