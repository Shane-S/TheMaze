using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AssignmentThree
{
    public class Flashlight
    {
        private Texture2D _lightShape;
        private float _intensity;
        private bool _isOn;
        private float _distance;
        private float _decay;
        private Color _colour;
        private Effect _lightEffect;

        /// <summary>
        /// Creates a flashlight effect originating from the camera with the
        /// specified radii and intensity.
        /// </summary>
        /// <param name="radius">The total radius of the beam.</param>
        /// <param name="intensity">The beam's intensity, between 0 and 1 (0 being invisible
        /// and 1 being the strongest possible light).</param>
        public Flashlight(float distance, float intensity, float decay, Color colour, Effect lightEffect)
        {
            _lightEffect = lightEffect;
            Decay = decay;
            Distance = distance;
            Intensity = intensity;
            Colour = colour;
        }

        /// <summary>
        /// Gets or sets the beam's intensity.
        /// </summary>
        public float Intensity { get { return _intensity; } 
            set 
            { 
                _intensity = value;
                _lightEffect.Parameters["DiffuseIntensity"].SetValue(value);
            } 
        }

        /// <summary>
        /// The distance over which the light will shine.
        /// </summary>
        public float Distance { get { return _distance; } 
            set 
            { 
                _distance = value; 
                _lightEffect.Parameters["LightDistance"].SetValue(value);
            } 
        }

        public float Decay { get { return _decay; }
            set
            {
                _decay = value;
                _lightEffect.Parameters["LightDecayExponent"].SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the light's colour.
        /// </summary>
        public Color Colour { get { return _colour; }
            set
            {
                _colour = value;
                _lightEffect.Parameters["LightColour"].SetValue(value.ToVector4());
            }
        }

        /// <summary>
        /// Tells whether the flashlight is on.
        /// </summary>
        public bool IsOn { get { return _isOn; } }

        /// <summary>
        /// Turns the flashlight on or off.
        /// </summary>
        public void power()
        {
            _isOn = !_isOn;
            _lightEffect.Parameters["FlashlightOn"].SetValue(_isOn);
        }
    }
}
