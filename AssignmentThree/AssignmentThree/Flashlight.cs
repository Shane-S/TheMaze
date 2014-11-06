using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Lab3
{
    public class Flashlight
    {
        private float _innerRadius;
        private float _outerRadius;
        private float _spaceRadius;
        private float _intensity;
        private bool _isOn;
        private Effect _lightEffect;

        /// <summary>
        /// Creates a flashlight effect originating from the camera with the
        /// specified radii and intensity.
        /// </summary>
        /// <param name="radius">The total radius of the beam.</param>
        /// <param name="intensity">The beam's intensity, between 0 and 1 (0 being invisible
        /// and 1 being the strongest possible light).</param>
        public Flashlight(float radius, float intensity, Effect lightEffect)
        {
            _outerRadius = radius;
            _spaceRadius = 2 * _outerRadius / 3;
            _innerRadius = _outerRadius / 3;
            _intensity = intensity;
            _lightEffect = lightEffect;
        }

        /// <summary>
        /// Gets or sets the total radius of the beam in pixels.
        /// </summary>
        public float Radius
        {
            get { return _outerRadius; }
            set
            {
                _outerRadius = value;
                _spaceRadius = 2 * _outerRadius / 3;
                _innerRadius = _outerRadius / 3;
            }
        }

        /// <summary>
        /// Gets or sets the beam's intensity, a value between 0 and 1.
        /// </summary>
        public float Intensity { get { return _intensity; } set { _intensity = value; } }

        /// <summary>
        /// Turns the flashlight on or off.
        /// </summary>
        public void power()
        {
            _isOn = !_isOn;
        }
    }
}
