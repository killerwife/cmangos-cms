'use client'

import { useRouter } from 'next/navigation'
import { useCookies } from 'react-cookie';
import { useState, useEffect } from 'react';
import QRCode from "react-qr-code";
import { jwtDecode } from 'jwt-decode'
import { env } from 'next-runtime-env';

export interface JwtCmangos {
    sub: string,
    name: string, // datetime
}

const fetchAuthenticatorKey = async (accessToken: string, failureCallback: Function) => {
    const NEXT_PUBLIC_API = env('NEXT_PUBLIC_API');
    var result = await fetch(NEXT_PUBLIC_API + '/generatetokensecret', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer: ' + accessToken
        },
    })
        .then(async (response) => {
            if (response.status == 401) {
                failureCallback()
                throw new Error('Unauthorized');
            }
            else if (response.status == 400) {
                failureCallback()
                throw new Error('Something failed while requesting token secret');
            }
            return response.text()
        })
        .then((r: string) => {
            return r
        })

    return result
}

export default function AuthenticatorAdd() {
    const [secretKey, setSecretKey] = useState<string>("")
    const router = useRouter()
    const [cookies] = useCookies(['access-token'])
    const [totp, setTotp] = useState('')
    const [sub, setSub] = useState('')
    const [totpError, setTotpError] = useState('')
    const [isLoading, setIsLoading] = useState<boolean>(false)
    const NEXT_PUBLIC_API = env('NEXT_PUBLIC_API');

    const getSub = () => {
        let token = jwtDecode<JwtCmangos>(cookies['access-token'])
        if (token.name == null)
            return ""
        return token.name.toString()
    }

    useEffect(() => {
        if (isLoading)
            return

        setSub(getSub())

        const fetchSecret = async () => {
            setIsLoading(true)
            let secretKey = await fetchAuthenticatorKey(cookies['access-token'], () => { router.push('/login') })
            setSecretKey(secretKey)
        }

        fetchSecret()
    }, [])

    const onAuthenticatorAdd = () => {
        if ('' == totp) {
            setTotpError("Please enter a totp token from your authenticator app after adding")
            return
        }

        fetch(NEXT_PUBLIC_API + '/addauthenticator', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer: ' + cookies['access-token'],
            },
            body: JSON.stringify({ token: totp }),
        })
            .then(async (response) => {
                if (response.status == 401) {
                    router.push('/login')
                    throw new Error('Unauthorized');
                }
                else if (response.status == 400) {
                    setTotpError('Token does not match secret')
                    throw new Error('Token does not match secret');
                }
                router.push('/account/addauthenticator/success')
            })            
    }

    return (
        <div>
            <h1>Authenticator removal</h1>
            <label>{totpError}</label>
            <div style={{ height: "auto", margin: "0 auto", maxWidth: 128, width: "100%" }}>
                <QRCode
                    size={256}
                    style={{ height: "auto", maxWidth: "100%", width: "100%" }}
                    value={"otpauth://totp/" + sub + "?secret=" + secretKey + "&issuer=cMaNGOS"}
                    viewBox={`0 0 256 256`}
                />
            </div>
            <br />
            <br />
            <label>Or enter manually: {secretKey}</label>
            <br />
            <div className={'inputContainer'}>
                <input
                    value={totp}
                    placeholder="Enter token from authenticator app"
                    onChange={(ev) => setTotp(ev.target.value)}
                    className={'inputBox'}
                />
            </div>
            <br />
            <div className={'inputContainer'}>
                <input className={'inputButton'} type="button" onClick={onAuthenticatorAdd} value={'Add authenticator'} />
            </div>
        </div>
    );
}