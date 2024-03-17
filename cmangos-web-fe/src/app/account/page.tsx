'use client'

import { useRouter } from 'next/navigation'
import { useCookies } from 'react-cookie';
import { useState, useEffect } from 'react';
import Link from "next/link";

export interface userInfo {
    email: string,
    hasAuthenticator: boolean,
}

const getUserInfo = async (accessToken: string, failureCallback: Function) => {
    var result = await fetch('https://localhost:7191/userinfo', {
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
            return response.json()
        })
            .then((r: userInfo) => {
                return r
            })

    return result
}

export default function Account() {
    const router = useRouter()
    const [cookies] = useCookies(['access-token'])
    const [isUsingAuthenticator, setUsingAuthenticator] = useState<boolean>()
    const [email, setEmail] = useState<string>()

    const onChangePassword = () => {
        router.push('/account/changepassword')
    }

    const onAddAuthenticator = () => {
        router.push('/account/addauthenticator')
    }

    const onRemoveAuthenticator = () => {
        router.push('/account/removeauthenticator')
    }

    useEffect(() => {
        if (cookies['access-token'] == null)
            return;

        const fetchUserInfo = async () => {
            let userInfo = await getUserInfo(cookies['access-token'], () => { router.push('/login') })
            setEmail(userInfo.email)
            setUsingAuthenticator(userInfo.hasAuthenticator)
        }

        fetchUserInfo()
    }, [])

    if (cookies['access-token'] == null) {
        return (
            <div><label>Please login</label> <Link href="/login">Login</Link></div>
        );
    }

    if (isUsingAuthenticator) {
        return (
            <main>
                <div className={'inputContainer'}>
                    <input className={'inputButton'} type="button" onClick={onChangePassword} value={'Change password'} />
                </div>
                <div className={'inputContainer'}>
                    <input className={'inputButton'} type="button" onClick={onRemoveAuthenticator} value={'Remove authenticator'} />
                </div>
            </main>
        );
    }

    return (
        <main>
            <div className={'inputContainer'}>
                <input className={'inputButton'} type="button" onClick={onChangePassword} value={'Change password'} />
            </div>
            <div className={'inputContainer'}>
                <input className={'inputButton'} type="button" onClick={onAddAuthenticator} value={'Add authenticator'} />
            </div>
        </main>
    );
}

