using Godot;
using System;

public partial class EnemyClass : Node3D
{

    [Export] private NodePath AnimationPlayerPath = "EnemigoP/AnimationPlayer";
    [Export] public string WalkAnimationName = "armatureAction_002";
    private AnimationPlayer _animPlayer;
    [Export] public float Hp = 20;
    [Export] private int Reward = 10;
    [Export] private float MeshHeight = 2.0f;
    [Export] public float SpeedMS = 3.0f;
    private PathFollow3D LevelPathFollow;

    //Señales
    [Signal]
    public delegate void OnEnemyDiedEventHandler(int price);

    [Signal]
    public delegate void OnEnemyFinishPathEventHandler();



    //Debuff variables
    private bool IsBurned = false;
    private float BurnedTimer = 0.0f;
    private float BurnedDamage = 0;

    private bool IsPoisoned = false;
    private float PoisonedTimer = 0.0f;
    private float PoisonedPercentaje = 0.0f;

    private bool IsParalized = false;
    private float ParalizedTimer = 0.0f;
    private float ParalizedPercentaje = 1.0f;

    private float _baseHp;
    private float _baseSpeed;
    private int _baseReward;

    public override void _Ready()
    {
        // Buscamos al padre. Si no es un PathFollow3D, esto dará error (y nos avisa si lo montamos mal)
        LevelPathFollow = GetParent() as PathFollow3D;

        _baseHp = Hp;
        _baseSpeed = SpeedMS;
        _baseReward = Reward;

        _animPlayer = GetNodeOrNull<AnimationPlayer>(AnimationPlayerPath);
        if (_animPlayer != null)
        {
            //GD.Print($"Animación cargada correctamente desde: {AnimationPlayerPath}");
            _animPlayer.Play(WalkAnimationName);
        }

        if (LevelPathFollow == null)
        {
            //GD.PrintErr("¡ERROR! El enemigo no está dentro de un PathFollow3D.");
        }

        // IMPORTANTE: Para que la rotación sea suave y no vibre
        LevelPathFollow.Loop = false; // Para que no vuelva al inicio al terminar
        LevelPathFollow.RotationMode = PathFollow3D.RotationModeEnum.Oriented;
        this.Position = new Vector3(0, MeshHeight / 2.0f, 0); // Ajustamos la altura del mesh
    }

    public override void _Process(double delta)
    {
        if (LevelPathFollow == null) return;

        if (_animPlayer != null)
        {
            _animPlayer.SpeedScale = ParalizedPercentaje;
        }

        if (IsBurned || IsParalized || IsPoisoned)
        {
            TakeDamageFromSecondaryEffect((float)delta);
        }

        // Simplemente avanzamos el "progreso" en metros
        LevelPathFollow.Progress += SpeedMS * ParalizedPercentaje * (float)delta;

        // Si el progreso es mayor o igual a la longitud total del camino...
        // Ratio 1.0 significa el final del camino (100%)
        if (LevelPathFollow.ProgressRatio >= 1.0f)
        {
            ReachEndPoint();
        }
    }

    private void ReachEndPoint()
    {
        //GD.Print("Enemigo llegó a la base.");
        // Aquí restarías vidas
        EmitSignal(SignalName.OnEnemyFinishPath);
        QueueFreeParent(); // Borramos el padre (el PathFollow) para que se vaya todo
    }

    private void QueueFreeParent()
    {
        if (LevelPathFollow != null)
        {
            LevelPathFollow.QueueFree();
        }
    }

    public void TakeDamage(float damage)
    {
        Hp -= damage;
        //GD.Print("Enemy took " + damage + " damage. Remaining HP: " + Hp);
        if (Hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //GD.Print("Enemy died.");
        EmitSignal(SignalName.OnEnemyDied, Reward);
        QueueFree();
    }

    public void ApplySecondaryEffect(SecondaryEffectEnum effect, int level)
    {
        switch (effect)
        {
            case SecondaryEffectEnum.Posion:
                if (IsPoisoned)
                {
                    PoisonedTimer = 0.0f;
                    PoisonedPercentaje += 0.1f * level;
                }
                else
                {
                    IsPoisoned = true;
                    PoisonedTimer = 0.0f;
                    PoisonedPercentaje = 0.1f * level;
                }
                break;
            case SecondaryEffectEnum.Fire:
                if (IsBurned)
                {
                    BurnedTimer = 0.0f;
                    BurnedDamage = Math.Max(BurnedDamage, 5 * level);
                }
                else
                {
                    IsBurned = true;
                    BurnedTimer = 0.0f;
                    BurnedDamage = 5 * level;
                }
                break;

            case SecondaryEffectEnum.Ice:
                if (IsParalized)
                {
                    ParalizedTimer = 0.0f;
                    ParalizedPercentaje = Math.Max(ParalizedPercentaje, 0.5f / level);
                }
                else
                {
                    IsParalized = true;
                    ParalizedTimer = 0.0f;
                    ParalizedPercentaje = 0.5f / level;
                }
                break;
        }
    }

    private void TakeDamageFromSecondaryEffect(float delta)
    {
        if (IsPoisoned)
        {
            int previousInt = Mathf.FloorToInt(PoisonedTimer);
            PoisonedTimer += delta;
            int afterInt = Mathf.FloorToInt(PoisonedTimer);
            if (previousInt != afterInt)
            {
                float damage = Hp * PoisonedPercentaje;
                TakeDamage(damage);
            }
            if (PoisonedTimer > 6.0f)
            {
                IsPoisoned = false;
                PoisonedPercentaje = 0.0f;
                PoisonedTimer = 0.0f;
            }
        }

        if (IsBurned)
        {
            int previousInt = Mathf.FloorToInt(BurnedTimer);
            BurnedTimer += delta;
            int afterInt = Mathf.FloorToInt(BurnedTimer);
            if (previousInt != afterInt)
            {
                TakeDamage(BurnedDamage);
            }
            if (BurnedTimer > 6.0f)
            {
                IsBurned = false;
                BurnedDamage = 0.0f;
                BurnedTimer = 0.0f;
            }
        }

        if (IsParalized)
        {
            ParalizedTimer += delta;
            if (ParalizedTimer > 3.0f)
            {
                IsParalized = false;
                ParalizedTimer = 0.0f;
                ParalizedPercentaje = 1.0f;
            }
        }
    }

    public void SetDifficulty(int difficultyIndex)
{
    // --- CORRECCIÓN: SEGURIDAD DE INICIALIZACIÓN ---
    // Si _baseHp es 0, significa que SetDifficulty se llamó antes que _Ready.
    // Guardamos los valores actuales del inspector como base ahora mismo.
    if (_baseHp == 0) 
    {
        _baseHp = Hp;
        _baseSpeed = SpeedMS;
        _baseReward = Reward;
    }
    // -----------------------------------------------

    // Reiniciamos a los valores base antes de aplicar multiplicadores
    Hp = _baseHp;
    SpeedMS = _baseSpeed;
    Reward = _baseReward;

    switch (difficultyIndex)
    {
        case 1: // FÁCIL
            // Ya se asignaron los valores base arriba, no hace falta hacer nada
            break;

        case 2: // MEDIA
            Hp = _baseHp * 1.5f;          
            SpeedMS = _baseSpeed * 1.1f;  
            Reward = Mathf.RoundToInt(_baseReward * 1.3f);
            break;

        case 3: // DIFÍCIL
            Hp = _baseHp * 2.5f;          
            SpeedMS = _baseSpeed * 1.25f; 
            Reward = Mathf.RoundToInt(_baseReward * 1.8f);
            break;
    }
}

}
