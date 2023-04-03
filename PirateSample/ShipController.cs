using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Pirates
{
    public class ShipController : MonoBehaviour
    {
        public float _MaxSpeed = 10f;
        public float _MaxSpeedInTurning = 5f;
        public float _ManeuverSpeed = 0.5f;
        public float _AccelerationSpeed = 0.5f;

        private bool    _HasTargetPos = false;
        private Vector3 _TargetPosition;
        private float   _CurrentSpeed = 1f;
        private float   _CurrentManeuverSpeed = 1f;
        private float   _CurrentMaxSpeed = 1f;
        private float   _CurrentMaxManeuverSpeed = 1f;

        private void Start()
        {
            _CurrentMaxSpeed = _MaxSpeed;
        }

        private void Update()
        {
            _CurrentManeuverSpeed = Mathf.Lerp(_CurrentManeuverSpeed, _CurrentMaxManeuverSpeed, 2f * Time.deltaTime);
            _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, _CurrentMaxSpeed, _CurrentManeuverSpeed * Time.deltaTime);

            transform.Translate(0, 0, _CurrentSpeed * Time.deltaTime);

            if (_HasTargetPos)
            {
                float distance = Vector3.Distance(transform.position, _TargetPosition);

                if (distance < 10f)
                {
                    _HasTargetPos = false;
                }
                else
                {
                    Vector3 direction = (_TargetPosition - transform.position).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, direction, 0.5f * Time.deltaTime);

                    float dot = Vector3.Dot(direction, transform.forward);
                    if (dot < 0.9f)
                    {
                        _CurrentMaxSpeed = _MaxSpeedInTurning;
                        _CurrentMaxManeuverSpeed = _ManeuverSpeed;
                    }
                    else
                    {
                        _CurrentMaxSpeed = _MaxSpeed;
                        _CurrentMaxManeuverSpeed = _AccelerationSpeed;
                    }
                }
            }
            else
            {
                _CurrentMaxManeuverSpeed = _AccelerationSpeed;
                _CurrentMaxSpeed = _MaxSpeed;
            }
        }

        public void SetTargetPosition(Vector3 pPosition)
        {
            _HasTargetPos = true;
            _TargetPosition = pPosition;
            _TargetPosition.y = 0f;
        }
    }
}
